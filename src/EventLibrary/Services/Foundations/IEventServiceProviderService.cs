using EventLibrary.Models;

namespace EventLibrary.Services.Foundations;

internal interface IEventServiceProviderService
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message);
    ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}
