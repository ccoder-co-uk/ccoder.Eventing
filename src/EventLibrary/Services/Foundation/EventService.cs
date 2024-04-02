using EventLibrary.Brokers.Interfaces;
using EventLibrary.Services.Foundation.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Services.Foundation
{
    public class EventService<T> : IEventService<T>
    {
        readonly IEventBroker<T> eventStorageBroker;
        private readonly ILogger<EventService<T>> log;

        public EventService(IEventBroker<T> eventStorageBroker, ILogger<EventService<T>> log)
        {
            this.eventStorageBroker = eventStorageBroker;
            this.log = log;
        }

        public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler) =>
            eventStorageBroker.ListenToEvent(name, handler);

        public async ValueTask RaiseEventAsync(string name, IServiceProvider serviceProvider, T message)
        {
            try
            {
                IEnumerable<Func<IServiceProvider, T, ValueTask>> handlers =
                    eventStorageBroker.GetHandlers(name);

                foreach (var handler in handlers)
                    await handler.Invoke(serviceProvider, message);
            }
            catch (Exception ex) 
            {
                log.LogError($"Exception thrown whilst raising {name} event", ex);
                throw;
            }
        }
    }
}