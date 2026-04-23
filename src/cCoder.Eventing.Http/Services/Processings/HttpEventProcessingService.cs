using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Http.Services.Processings;

internal class HttpEventProcessingService(IHttpEventService httpEventService)
    : IHttpEventProcessingService
{
    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
            httpEventService.ListenToEvent(name, handler);

    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default) =>
            httpEventService.RaiseEventAsync(name, message, cancellationToken);

    public async ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default)
    {
        foreach (EventMessage<T> message in messages ?? [])
        {
            await RaiseEventAsync(name, message, cancellationToken);
        }
    }

    public ValueTask ReceiveEventAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default) =>
            httpEventService.ReceiveEventAsync(message, cancellationToken);
}
