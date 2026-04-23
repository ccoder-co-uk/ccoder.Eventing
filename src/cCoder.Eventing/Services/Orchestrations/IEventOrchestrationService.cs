using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Orchestrations;

internal interface IEventOrchestrationService
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    void ListenToEvent<T, TService>(string name, Func<TService, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message);
    ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}
