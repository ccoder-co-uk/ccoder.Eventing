// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Sql.Services.Foundations;

internal interface ISqlEventService
{
    Task ProcessAsync(IServiceProvider scopedProvider, CancellationToken cancellationToken);

    Task<Guid> RaiseEventAsync<T>(string name, EventMessage<T> eventMessage, CancellationToken cancellationToken);

    Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(int count, CancellationToken cancellationToken);

    Task<Guid?> RequeueAsync(Guid failedId, CancellationToken cancellationToken);

    Task<bool> RemoveAsync(Guid failedId, CancellationToken cancellationToken);
}