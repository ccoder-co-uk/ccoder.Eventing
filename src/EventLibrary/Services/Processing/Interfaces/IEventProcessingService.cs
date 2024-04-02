using EventLibrary.Objects;

namespace EventLibrary.Services.Processing.Interfaces;

public interface IEventProcessingService<T>
{
    void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync(string name, EventMessage<T> data);
}