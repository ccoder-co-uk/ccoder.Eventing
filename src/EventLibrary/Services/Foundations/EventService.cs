using EventLibrary.Brokers;
using EventLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Services.Foundations;

internal class EventService<T>(
        IServiceProviderBroker serviceProviderBroker,
        ILogger<EventService<T>> log) 
            : IEventService<T>
{
    public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler)
    {
        try
        {
            serviceProviderBroker.GetService<IEventBroker<T>>()
                .ListenToEvent(name, handler);
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst listening to {Name} event\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }

    public async ValueTask RaiseEventAsync(string name, EventMessage<T> message)
    {
        try
        {
            using IServiceScope scope = serviceProviderBroker.GetScopeForEvent(message);

            IEnumerable<Func<IServiceProvider, T, ValueTask>> handlers =
                serviceProviderBroker.GetService<IEventBroker<T>>()
                    .GetHandlers(name);

            foreach (Func<IServiceProvider, T, ValueTask> handler in handlers)
                await handler.Invoke(scope.ServiceProvider, message.Data);
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst raising {Name} event\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }
}
