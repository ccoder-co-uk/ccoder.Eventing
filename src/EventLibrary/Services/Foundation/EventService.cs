using EventLibrary.Brokers.Interfaces;
using EventLibrary.Services.Foundation.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Services.Foundation;

/// <summary>
/// 
/// </summary>
/// <typeparam name="T"></typeparam>
public class EventService<T> : IEventService<T>
{
    readonly IEventBroker<T> eventStorageBroker;
    private readonly ILogger<EventService<T>> log;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="eventStorageBroker"></param>
    /// <param name="log"></param>
    public EventService(IEventBroker<T> eventStorageBroker, ILogger<EventService<T>> log)
    {
        this.eventStorageBroker = eventStorageBroker;
        this.log = log;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="handler"></param>
    public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler) =>
        eventStorageBroker.ListenToEvent(name, handler);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="name"></param>
    /// <param name="serviceProvider"></param>
    /// <param name="message"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
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
            log.LogError(ex, "Exception thrown whilst raising {Name} event\n{Message}\n{StackTrace}", name, ex.Message, ex.StackTrace);
            throw new InvalidOperationException("Eventing is unable to raise event, see inner exception for details", ex);
        }
    }
}