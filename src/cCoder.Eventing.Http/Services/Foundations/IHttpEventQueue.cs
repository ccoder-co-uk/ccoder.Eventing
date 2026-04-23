using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Services.Foundations;

internal interface IHttpEventQueue
{
    ValueTask EnqueueAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<HttpEventMessage> ReadAllAsync(
        CancellationToken cancellationToken = default);
}
