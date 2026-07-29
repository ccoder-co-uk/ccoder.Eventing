// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Http.Dependencies;

internal class HttpEventProcessingServiceDependency(
    IHttpEventService httpEventService)
    : IHttpEventProcessingService
{
    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        httpEventService.ListenToEvent(name:name, handler:handler);

    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default) =>
        httpEventService.RaiseEventAsync(name:name, message:message, cancellationToken:cancellationToken);

    public async ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default)
    {
        foreach (EventMessage<T> message in messages ?? [])
        {
            await RaiseEventAsync(name:name, message:message, cancellationToken:cancellationToken);
        }
    }

    public ValueTask ReceiveEventAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default) =>
        httpEventService.ReceiveEventAsync(message:message, cancellationToken:cancellationToken);
}