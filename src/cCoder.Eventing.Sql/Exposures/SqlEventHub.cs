// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Eventing.Models;
using cCoder.Eventing.Sql.Services.Processings;

namespace cCoder.Eventing.Sql;

internal sealed class SqlEventHub(ISqlEventProcessingService processingService)
    : ISqlEventHub
{
    public ValueTask<Guid> RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default) =>
        processingService.RaiseEventAsync(
            name: name,
            eventMessage: message,
            cancellationToken: cancellationToken);

    public Task RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default) =>
        processingService.RaiseEventsAsync(
            name: name,
            eventMessages: messages,
            cancellationToken: cancellationToken);
}