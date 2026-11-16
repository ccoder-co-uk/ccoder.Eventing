// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Sql.Services.Processings;

internal interface ISqlEventProcessingService
{
    Task ProcessAsync(IServiceProvider scopedProvider, CancellationToken cancellationToken);

    ValueTask<Guid> RaiseEventAsync<T>(string name, EventMessage<T> eventMessage, CancellationToken cancellationToken);

    Task RaiseEventsAsync<T>(string name, EventMessage<T>[] eventMessages, CancellationToken cancellationToken);

    Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(int count, CancellationToken cancellationToken);

    Task<Guid?> RequeueAsync(Guid failedId, CancellationToken cancellationToken);

    Task<bool> RemoveAsync(Guid failedId, CancellationToken cancellationToken);
}