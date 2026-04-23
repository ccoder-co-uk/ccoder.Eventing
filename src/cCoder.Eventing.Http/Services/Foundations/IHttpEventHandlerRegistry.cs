using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Services.Foundations;

internal interface IHttpEventHandlerRegistry
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);

    IReadOnlyCollection<HttpEventSubscription> GetSubscriptions(string name);
}
