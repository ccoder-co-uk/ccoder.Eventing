// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Extensions;
using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Services.Foundations;

internal sealed partial class EventService<T>(
        IServiceProviderBroker serviceProviderBroker,
        ILogger<EventService<T>> log) 
            : IEventService<T>
{
    private readonly IDictionary<
        string,
        ICollection<Func<IServiceProvider, T, ValueTask>>> handlersByName =
            new Dictionary<
                string,
                ICollection<Func<IServiceProvider, T, ValueTask>>>();

    public void ListenToEvent(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            try
            {
                GetOrCreateHandlers(name: name)
                    .Add(item: handler);
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
                using IServiceScope scope =
                    serviceProviderBroker.GetScopeForEvent(message: message);

                IEnumerable<Func<IServiceProvider, T, ValueTask>> handlers =
                    GetOrCreateHandlers(name: name);

                await EventDispatchExtensions.HandleHandlersAsync(
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

    private ICollection<Func<IServiceProvider, T, ValueTask>> GetOrCreateHandlers(
        string name)
    {
        lock (handlersByName)
        {
            if (!handlersByName.TryGetValue(
                key: name,
                value: out ICollection<Func<IServiceProvider, T, ValueTask>> handlers))
            {
                handlers = new List<Func<IServiceProvider, T, ValueTask>>();
                handlersByName.Add(key: name, value: handlers);
            }

            return handlers;
        }
    }
}