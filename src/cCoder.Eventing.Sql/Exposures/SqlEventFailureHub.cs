// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Sql.Services.Processings;

namespace cCoder.Eventing.Sql;

internal sealed class SqlEventFailureHub(ISqlEventProcessingService processingService)
    : ISqlEventFailureHub
{
    public Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(
        int count,
        CancellationToken cancellationToken = default) =>
        processingService.GetFailedAsync(
            count: count,
            cancellationToken: cancellationToken);

    public Task<Guid?> RequeueAsync(
        Guid failedId,
        CancellationToken cancellationToken = default) =>
        processingService.RequeueAsync(
            failedId: failedId,
            cancellationToken: cancellationToken);

    public Task<bool> RemoveAsync(
        Guid failedId,
        CancellationToken cancellationToken = default) =>
        processingService.RemoveAsync(
            failedId: failedId,
            cancellationToken: cancellationToken);
}