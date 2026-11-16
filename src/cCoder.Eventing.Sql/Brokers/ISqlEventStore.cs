// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;

namespace cCoder.Eventing.Sql.Brokers;

internal interface ISqlEventStore
{
    ValueTask EnqueueAsync(EventMessageRecord eventMessageRecord, CancellationToken cancellationToken);

    Task<IReadOnlyList<EventMessageRecord>> ClaimAsync(
        int count,
        TimeSpan leaseDuration,
        CancellationToken cancellationToken);

    Task<bool> CompleteAsync(EventMessageRecord eventMessageRecord, CancellationToken cancellationToken);

    Task<bool> FailAsync(
        EventMessageRecord eventMessageRecord,
        string reason,
        CancellationToken cancellationToken);

    Task<Guid?> RetryAsync(
        EventMessageRecord eventMessageRecord,
        TimeSpan delay,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(
        int count,
        CancellationToken cancellationToken);

    Task<SqlEventQueueReport> GetQueueReportAsync(CancellationToken cancellationToken);

    Task<Guid?> RequeueFailedAsync(Guid failedId, CancellationToken cancellationToken);

    Task<bool> RemoveFailedAsync(Guid failedId, CancellationToken cancellationToken);
}