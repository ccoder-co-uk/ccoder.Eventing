using EventLibrary.Brokers.Interfaces;

namespace EventLibrary
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventBroker<T> : IEventBroker<T>
    {
        readonly IDictionary<string, ICollection<Func<IServiceProvider, T, ValueTask>>> functionBindings
            = new Dictionary<string, ICollection<Func<IServiceProvider, T, ValueTask>>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsListenedEvent(string name)
            => functionBindings.ContainsKey(name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Func<IServiceProvider, T, ValueTask>> GetHandlers(string name)
        {
            functionBindings.TryGetValue(name, out ICollection<Func<IServiceProvider, T, ValueTask>> value);
            return value ?? SetupEventForListening(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler)
        {
            var handlerSet = GetHandlers(name) as ICollection<Func<IServiceProvider, T, ValueTask>>;
            handlerSet.Add(handler);
            functionBindings[name] = handlerSet;
        }

        ICollection<Func<IServiceProvider, T, ValueTask>> SetupEventForListening(string name)
        {
            lock (functionBindings)
            {
                functionBindings.TryGetValue(name, out ICollection<Func<IServiceProvider, T, ValueTask>> value);

                if (value == null)
                    functionBindings.Add(name, new List<Func<IServiceProvider, T, ValueTask>>());

                return functionBindings[name];
            }
        }
    }
}