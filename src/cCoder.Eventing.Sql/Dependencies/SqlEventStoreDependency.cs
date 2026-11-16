// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data;
using cCoder.Data.Models.Eventing;
using Microsoft.EntityFrameworkCore;


namespace cCoder.Eventing.Sql.Dependencies;

internal sealed class SqlEventStoreDependency(ICoreContextFactory contextFactory)
    : ISqlEventStorage
{
    private CoreDataContext CreateDbContext() => contextFactory.CreateCoreContext();

    public async ValueTask EnqueueAsync(
        EventMessageRecord eventMessageRecord,
        CancellationToken cancellationToken)
    {
        using CoreDataContext context = CreateDbContext();
        context.EventMessages.Add(eventMessageRecord);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EventMessageRecord>> ClaimAsync(
        int count,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken)
    {
        if (count < 1)
            throw new ArgumentOutOfRangeException(nameof(count));

        using CoreDataContext context = CreateDbContext();
        DbConnection connection = context.Database.GetDbConnection();
        await connection.OpenAsync(cancellationToken);

        try
        {
            using DbCommand command = connection.CreateCommand();
            command.CommandText = """
                ;WITH Ready AS (
                    SELECT TOP (@Count) *
                    FROM [Eventing].[EventMessages] WITH (ROWLOCK, UPDLOCK, READPAST)
                    WHERE [AvailableOn] <= @Now
                      AND ([LeaseUntil] IS NULL OR [LeaseUntil] < @Now)
                    ORDER BY [CreatedOn], [Id]
                )
                UPDATE Ready SET [LeaseUntil] = @LeaseUntil
                OUTPUT inserted.[Id], inserted.[CreatedOn], inserted.[EventName],
                       inserted.[Data], inserted.[AvailableOn],
                       inserted.[LeaseUntil], inserted.[Attempt];
                """;

            DateTimeOffset now = DateTimeOffset.UtcNow;
            AddParameter(command, "@Count", count);
            AddParameter(command, "@Now", now);
            AddParameter(command, "@LeaseUntil", now.Add(leaseDuration));

            List<EventMessageRecord> messages = [];
            using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

            while (await reader.ReadAsync(cancellationToken))
            {
                messages.Add(new EventMessageRecord
                {
                    Id = reader.GetGuid(0),
                    CreatedOn = reader.GetFieldValue<DateTimeOffset>(1),
                    EventName = reader.GetString(2),
                    Data = reader.GetString(3),
                    AvailableOn = reader.GetFieldValue<DateTimeOffset>(4),
                    LeaseUntil = reader.GetFieldValue<DateTimeOffset>(5),
                    Attempt = reader.GetInt32(6)
                });
            }

            return messages;
        }
        finally
        {
            await connection.CloseAsync();
        }
    }

    public async Task<bool> CompleteAsync(
        EventMessageRecord eventMessageRecord,
        CancellationToken cancellationToken)
    {
        using CoreDataContext context = CreateDbContext();
        int affected = await context.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM [Eventing].[EventMessages] WHERE [Id] = {eventMessageRecord.Id} AND [LeaseUntil] = {eventMessageRecord.LeaseUntil}",
            cancellationToken);
        return affected == 1;
    }

    public async Task<bool> FailAsync(
        EventMessageRecord eventMessageRecord,
        string reason,
        CancellationToken cancellationToken)
    {
        using CoreDataContext context = CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

        int inserted = await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
                INSERT INTO [Eventing].[FailedEventMessages]
                    ([Id], [CreatedOn], [EventName], [Data], [AvailableOn],
                     [LeaseUntil], [Attempt], [FailureReason])
                SELECT [Id], [CreatedOn], [EventName], [Data], [AvailableOn],
                       [LeaseUntil], [Attempt], {reason}
                FROM [Eventing].[EventMessages] WITH (UPDLOCK, HOLDLOCK)
                WHERE [Id] = {eventMessageRecord.Id} AND [LeaseUntil] = {eventMessageRecord.LeaseUntil}
                """,
            cancellationToken);

        if (inserted != 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            return false;
        }

        int deleted = await context.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM [Eventing].[EventMessages] WHERE [Id] = {eventMessageRecord.Id} AND [LeaseUntil] = {eventMessageRecord.LeaseUntil}",
            cancellationToken);

        if (deleted != 1)
            throw new InvalidOperationException("Failed event was copied but could not be removed from the queue.");

        await transaction.CommitAsync(cancellationToken);
        return true;
    }

    public async Task<Guid?> RetryAsync(
        EventMessageRecord eventMessageRecord,
        TimeSpan delay,
        CancellationToken cancellationToken)
    {
        if (delay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(delay));

        using CoreDataContext context = CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        Guid newId = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;

        int inserted = await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
                INSERT INTO [Eventing].[EventMessages]
                    ([Id], [CreatedOn], [EventName], [Data], [AvailableOn],
                     [LeaseUntil], [Attempt])
                SELECT {newId}, {now}, [EventName], [Data], {now.Add(delay)},
                       NULL, [Attempt] + 1
                FROM [Eventing].[EventMessages] WITH (UPDLOCK, HOLDLOCK)
                WHERE [Id] = {eventMessageRecord.Id} AND [LeaseUntil] = {eventMessageRecord.LeaseUntil}
                """,
            cancellationToken);

        if (inserted != 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            return null;
        }

        int deleted = await context.Database.ExecuteSqlInterpolatedAsync(
            $"DELETE FROM [Eventing].[EventMessages] WHERE [Id] = {eventMessageRecord.Id} AND [LeaseUntil] = {eventMessageRecord.LeaseUntil}",
            cancellationToken);

        if (deleted != 1)
            throw new InvalidOperationException("Retried event was inserted but the original could not be removed.");

        await transaction.CommitAsync(cancellationToken);
        return newId;
    }

    public async Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(
        int count,
        CancellationToken cancellationToken)
    {
        if (count is < 1 or > 1000)
            throw new ArgumentOutOfRangeException(nameof(count));

        using CoreDataContext context = CreateDbContext();
        return await context.FailedEventMessages
            .AsNoTracking()
            .OrderByDescending(message => message.CreatedOn)
            .Take(count)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<SqlEventQueueReport> GetQueueReportAsync(
        CancellationToken cancellationToken)
    {
        using CoreDataContext context = CreateDbContext();
        DateTimeOffset now = DateTimeOffset.UtcNow;

        var active = await context.EventMessages
            .AsNoTracking()
            .GroupBy(message => message.EventName)
            .Select(group => new
            {
                Name = group.Key,
                Queued = group.LongCount(message =>
                    message.LeaseUntil == null || message.LeaseUntil <= now),
                Processing = group.LongCount(message => message.LeaseUntil > now)
            })
            .ToArrayAsync(cancellationToken);

        var failed = await context.FailedEventMessages
            .AsNoTracking()
            .GroupBy(message => message.EventName)
            .Select(group => new { Name = group.Key, Count = group.LongCount() })
            .ToArrayAsync(cancellationToken);

        Dictionary<string, SqlEventQueueSnapshot> snapshots =
            active.ToDictionary(
                keySelector: item => item.Name,
                elementSelector: item => new SqlEventQueueSnapshot
                {
                    Name = item.Name,
                    Queued = item.Queued,
                    Processing = item.Processing
                },
                comparer: StringComparer.Ordinal);

        foreach (var item in failed)
        {
            if (!snapshots.TryGetValue(item.Name, out SqlEventQueueSnapshot snapshot))
            {
                snapshot = new SqlEventQueueSnapshot { Name = item.Name };
                snapshots.Add(item.Name, snapshot);
            }

            snapshot.Failed = item.Count;
        }

        return new SqlEventQueueReport
        {
            Queues = snapshots.Values.OrderBy(snapshot => snapshot.Name).ToArray()
        };
    }

    public async Task<Guid?> RequeueFailedAsync(
        Guid failedId,
        CancellationToken cancellationToken)
    {
        using CoreDataContext context = CreateDbContext();
        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        Guid newId = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.UtcNow;

        int inserted = await context.Database.ExecuteSqlInterpolatedAsync(
            $"""
                INSERT INTO [Eventing].[EventMessages]
                    ([Id], [CreatedOn], [EventName], [Data], [AvailableOn],
                     [LeaseUntil], [Attempt])
                SELECT {newId}, {now}, [EventName], [Data], {now},
                       NULL, [Attempt] + 1
                FROM [Eventing].[FailedEventMessages] WITH (UPDLOCK, HOLDLOCK)
                WHERE [Id] = {failedId}
                """,
            cancellationToken);

        if (inserted != 1)
        {
            await transaction.RollbackAsync(cancellationToken);
            return null;
        }

        int removed = await context.FailedEventMessages
            .Where(message => message.Id == failedId)
            .ExecuteDeleteAsync(cancellationToken);

        if (removed != 1)
            throw new InvalidOperationException("Requeued event could not be removed from failures.");

        await transaction.CommitAsync(cancellationToken);
        return newId;
    }

    public async Task<bool> RemoveFailedAsync(
        Guid failedId,
        CancellationToken cancellationToken)
    {
        using CoreDataContext context = CreateDbContext();
        return await context.FailedEventMessages
            .Where(message => message.Id == failedId)
            .ExecuteDeleteAsync(cancellationToken) == 1;
    }

    private static void AddParameter(DbCommand command, string name, object value)
    {
        DbParameter parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }
}