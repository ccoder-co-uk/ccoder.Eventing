namespace EventLibrary.Brokers.Interfaces
{
    public interface IEventBroker<T>
    {
        IEnumerable<Func<IServiceProvider, T, ValueTask>> GetHandlers(string name);
        void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler);
    }
}