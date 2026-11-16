// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;

namespace cCoder.Eventing.Sql.Brokers;

internal sealed class SqlEventBroker(ISqlEventStorage dependency) : ISqlEventStore
{
    public ValueTask EnqueueAsync(EventMessageRecord eventMessageRecord, CancellationToken cancellationToken) =>
        dependency.EnqueueAsync(eventMessageRecord: eventMessageRecord, cancellationToken: cancellationToken);

    public Task<IReadOnlyList<EventMessageRecord>> ClaimAsync(int count, TimeSpan leaseDuration, CancellationToken cancellationToken) =>
        dependency.ClaimAsync(count: count, leaseDuration: leaseDuration, cancellationToken: cancellationToken);

    public Task<bool> CompleteAsync(EventMessageRecord eventMessageRecord, CancellationToken cancellationToken) =>
        dependency.CompleteAsync(eventMessageRecord: eventMessageRecord, cancellationToken: cancellationToken);

    public Task<bool> FailAsync(EventMessageRecord eventMessageRecord, string reason, CancellationToken cancellationToken) =>
        dependency.FailAsync(eventMessageRecord: eventMessageRecord, reason: reason, cancellationToken: cancellationToken);

    public Task<Guid?> RetryAsync(EventMessageRecord eventMessageRecord, TimeSpan delay, CancellationToken cancellationToken) =>
        dependency.RetryAsync(eventMessageRecord: eventMessageRecord, delay: delay, cancellationToken: cancellationToken);

    public Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(int count, CancellationToken cancellationToken) =>
        dependency.GetFailedAsync(count: count, cancellationToken: cancellationToken);

    public Task<SqlEventQueueReport> GetQueueReportAsync(CancellationToken cancellationToken) =>
        dependency.GetQueueReportAsync(cancellationToken: cancellationToken);

    public Task<Guid?> RequeueFailedAsync(Guid failedId, CancellationToken cancellationToken) =>
        dependency.RequeueFailedAsync(failedId: failedId, cancellationToken: cancellationToken);

    public Task<bool> RemoveFailedAsync(Guid failedId, CancellationToken cancellationToken) =>
        dependency.RemoveFailedAsync(failedId: failedId, cancellationToken: cancellationToken);
}