using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Http;

public interface IHttpEventHub
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);

    ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default);

    ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default);

    ValueTask ReceiveEventAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default);
}
