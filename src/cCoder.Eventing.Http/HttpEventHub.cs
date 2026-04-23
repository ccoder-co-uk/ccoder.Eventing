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
            httpEventProcessingService.ListenToEvent(name, handler);

    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default) =>
            httpEventProcessingService.RaiseEventAsync(name, message, cancellationToken);

    public ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default) =>
            httpEventProcessingService.RaiseEventsAsync(name, messages, cancellationToken);

    public ValueTask ReceiveEventAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default) =>
            httpEventProcessingService.ReceiveEventAsync(message, cancellationToken);
}
