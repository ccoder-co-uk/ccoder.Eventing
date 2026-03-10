using EventLibrary.Brokers;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Services.Foundations;

public class EventService<T> : IEventService<T>
{
    private readonly IEventBroker<T> eventStorageBroker;
    private readonly ILogger<EventService<T>> log;

    public EventService(
        IEventBroker<T> eventStorageBroker,
        ILogger<EventService<T>> log)
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

            foreach (Func<IServiceProvider, T, ValueTask> handler in handlers)
            {
                await handler.Invoke(serviceProvider, message);
            }
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst raising {Name} event\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw new InvalidOperationException(
                "Eventing is unable to raise event, see inner exception for details",
                ex);
        }
    }
}
