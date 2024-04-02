using EventLibrary.Objects;

namespace EventLibrary.Services.Foundation.Interfaces
{
    public interface IEventService<T>
    {
        void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler);
        ValueTask RaiseEventAsync(string name, IServiceProvider serviceProvider, T message);
    }
}