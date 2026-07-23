// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Services.Foundations;

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
                .ListenToEvent(name:name, handler:handler);
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
            using IServiceScope scope = serviceProviderBroker.GetScopeForEvent(message:message);

            IEnumerable<Func<IServiceProvider, T, ValueTask>> handlers =
                serviceProviderBroker.GetService<IEventBroker<T>>()
                    .GetHandlers(name:name);

            foreach (Func<IServiceProvider, T, ValueTask> handler in handlers)
                await handler.Invoke(arg1:scope.ServiceProvider, arg2:message.Data);
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