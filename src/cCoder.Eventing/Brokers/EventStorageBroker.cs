// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;

namespace cCoder.Eventing.Brokers;

internal class EventBroker<T> : IEventBroker<T>
{
    private readonly IDictionary<string, ICollection<Func<IServiceProvider, T, ValueTask>>> functionBindings =
        new Dictionary<string, ICollection<Func<IServiceProvider, T, ValueTask>>>();

    public IEnumerable<Func<IServiceProvider, T, ValueTask>> GetHandlers(string name)
    {
        functionBindings.TryGetValue(key:name, value:out ICollection<Func<IServiceProvider, T, ValueTask>> value);
        return value ?? SetupEventForListening(name:name);
    }

    public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler)
    {
        ICollection<Func<IServiceProvider, T, ValueTask>> handlerSet =
            GetHandlers(name:name) as ICollection<Func<IServiceProvider, T, ValueTask>>;

        handlerSet.Add(item:handler);
        functionBindings[name] = handlerSet;
    }

    private ICollection<Func<IServiceProvider, T, ValueTask>> SetupEventForListening(string name)
    {
        lock (functionBindings)
        {
            functionBindings.TryGetValue(key:name, value:out ICollection<Func<IServiceProvider, T, ValueTask>> value);

            if (value is null)
            {
                functionBindings.Add(key:name, value:new List<Func<IServiceProvider, T, ValueTask>>());
            }

            return functionBindings[name];
        }
    }
}