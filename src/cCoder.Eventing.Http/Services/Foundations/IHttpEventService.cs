using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Http.Services.Foundations;

internal interface IHttpEventService
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);

    ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default);

    ValueTask ReceiveEventAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default);
}
