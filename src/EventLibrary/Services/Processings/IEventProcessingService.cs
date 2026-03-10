using EventLibrary.Models;

namespace EventLibrary.Services.Processings;

public interface IEventProcessingService<T>
{
    void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync(string name, EventMessage<T> data);
}
