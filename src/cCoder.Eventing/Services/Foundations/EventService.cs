// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Dependencies;
using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Services.Foundations;

internal sealed partial class EventService<T>(
        IServiceProviderBroker serviceProviderBroker,
        ILogger<EventService<T>> log) 
            : IEventService<T>
{
    public void ListenToEvent(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            try
            {
                serviceProviderBroker
                    .GetService<IEventBroker<T>>()
                    .ListenToEvent(name:name, handler:handler);
            }
            catch (Exception ex)
            {
                log.LogError(
                    exception: ex,
                    message: "Exception thrown whilst listening to {Name} event\n{Message}\n{StackTrace}",
                    args: [name, ex.Message, ex.StackTrace]);

                throw;
            }
        });

    public ValueTask RaiseEventAsync(
        string name,
        EventMessage<T> message) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, message]);

            try
            {
            using IServiceScope scope = serviceProviderBroker.GetScopeForEvent(message:message);

            IEnumerable<Func<IServiceProvider, T, ValueTask>> handlers =
                serviceProviderBroker.GetService<IEventBroker<T>>()
                    .GetHandlers(name:name);

                await EventDispatchDependency.HandleHandlersAsync(
                    handlers: handlers,
                    serviceProvider: scope.ServiceProvider,
                    message: message.Data);
            }
            catch (Exception ex)
            {
                log.LogError(
                    exception: ex,
                    message: "Exception thrown whilst raising {Name} event\n{Message}\n{StackTrace}",
                    args: [name, ex.Message, ex.StackTrace]);

                throw;
            }
        });
}