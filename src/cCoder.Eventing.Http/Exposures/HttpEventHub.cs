// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Processings;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Http;

public class HttpEventHub : IHttpEventHub
{
    private readonly IHttpEventProcessingService httpEventProcessingService;

    internal HttpEventHub(IHttpEventProcessingService httpEventProcessingService) =>
        this.httpEventProcessingService = httpEventProcessingService;

    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        httpEventProcessingService.ListenToEvent(name:name, handler:handler);

    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default) =>
        httpEventProcessingService.RaiseEventAsync(name:name, message:message, cancellationToken:cancellationToken);

    public ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default) =>
        httpEventProcessingService.RaiseEventsAsync(name:name, messages:messages, cancellationToken:cancellationToken);

    public ValueTask ReceiveEventAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default) =>
        httpEventProcessingService.ReceiveEventAsync(message:message, cancellationToken:cancellationToken);
}