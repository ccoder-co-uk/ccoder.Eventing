using EventLibrary.Objects;

namespace EventLibrary
{
    public interface IEventHub
    {
        void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
        ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message);
        ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
    }
}