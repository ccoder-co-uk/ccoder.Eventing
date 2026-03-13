using EventLibrary.Models;

namespace EventLibrary.Services.Foundations;

internal interface IEventService<T>
{
    void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync(string name, EventMessage<T> message);
}
