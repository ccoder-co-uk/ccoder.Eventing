// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;

namespace cCoder.Eventing.Sql;

public interface ISqlEventFailureHub
{
    Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(
        int count,
        CancellationToken cancellationToken = default);

    Task<Guid?> RequeueAsync(
        Guid failedId,
        CancellationToken cancellationToken = default);

    Task<bool> RemoveAsync(
        Guid failedId,
        CancellationToken cancellationToken = default);
}