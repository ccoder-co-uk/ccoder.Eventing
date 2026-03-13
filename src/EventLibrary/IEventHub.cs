using EventLibrary.Models;

namespace EventLibrary;

public interface IEventHub
{
    void ListenToEvent<T, TService>(string name, Func<TService, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message);
    ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}