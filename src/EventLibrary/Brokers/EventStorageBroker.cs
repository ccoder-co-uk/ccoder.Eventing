using EventLibrary.Brokers;

namespace EventLibrary.Brokers;

internal class EventBroker<T> : IEventBroker<T>
{
    private readonly IDictionary<string, ICollection<Func<IServiceProvider, T, ValueTask>>> functionBindings =
        new Dictionary<string, ICollection<Func<IServiceProvider, T, ValueTask>>>();

    public IEnumerable<Func<IServiceProvider, T, ValueTask>> GetHandlers(string name)
    {
        functionBindings.TryGetValue(name, out ICollection<Func<IServiceProvider, T, ValueTask>> value);
        return value ?? SetupEventForListening(name);
    }

    public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler)
    {
        ICollection<Func<IServiceProvider, T, ValueTask>> handlerSet =
            GetHandlers(name) as ICollection<Func<IServiceProvider, T, ValueTask>>;

        handlerSet.Add(handler);
        functionBindings[name] = handlerSet;
    }

    private ICollection<Func<IServiceProvider, T, ValueTask>> SetupEventForListening(string name)
    {
        lock (functionBindings)
        {
            functionBindings.TryGetValue(name, out ICollection<Func<IServiceProvider, T, ValueTask>> value);

            if (value is null)
            {
                functionBindings.Add(name, new List<Func<IServiceProvider, T, ValueTask>>());
            }

            return functionBindings[name];
        }
    }
}
