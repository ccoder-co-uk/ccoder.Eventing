// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Processings;
using cCoder.Eventing.Brokers.Loggings;

namespace cCoder.Eventing.Services.Foundations;

internal sealed partial class EventServiceProviderService : IEventServiceProviderService
{
    private readonly List<object> services = [];
    private readonly IServiceProviderBroker serviceProviderBroker;
    private readonly ILoggingBroker log;

    public EventServiceProviderService(
        IServiceProviderBroker serviceProviderBroker,
        ILoggingBroker log)
    {
        this.serviceProviderBroker = serviceProviderBroker;
        this.log = log;
    }

    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            try
            {
                IEventProcessingService<T> typedEventService = GetEventService<T>();

                if (typedEventService is null)
                {
                    typedEventService = serviceProviderBroker.GetService<IEventProcessingService<T>>();
                    services.Add(item:typedEventService);
                }

                typedEventService.ListenToEvent(name:name, handler:handler);
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

    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, message]);

            try
            {
                await RaiseEventInternalAsync(name:name, message:message);
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

    public ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, messages]);

            try
            {
                foreach (EventMessage<T> message in messages)
                {
                    await RaiseEventInternalAsync(
                        name: name,
                        message: message);
                }
            }
            catch (Exception ex)
            {
                log.LogError(
                    exception: ex,
                    message: "Exception thrown whilst raising {Name} events\n{Message}\n{StackTrace}",
                    args: [name, ex.Message, ex.StackTrace]);

                throw;
            }
        });

    private async ValueTask RaiseEventInternalAsync<T>(
        string name,
        EventMessage<T> message)
    {
        ValidateRequest(name:name, message:message);

        IEventProcessingService<T> service = GetEventService<T>();

        if (service is null)
        {
            log.LogWarning(
                message: "{Name} event was raised, but no handler was configured for it",
                args: name);

            return;
        }

        await service.RaiseEventAsync(name:name, data:message);
    }

    private static void ValidateRequest<T>(string name, EventMessage<T> message)
    {
        if (name is null)
        {
            throw new InvalidOperationException("You must provide an event name when raising events.");
        }

        if (message is null)
        {
            throw new InvalidOperationException("You must provide a message when raising events.");
        }

        if (message.Data is null)
        {
            throw new InvalidOperationException("You must provide some message data when raising events.");
        }

        if (message.AuthInfo is null)
        {
            throw new InvalidOperationException("You must provide some message auth information when raising events.");
        }
    }

    private IEventProcessingService<T> GetEventService<T>() =>
        services
            .OfType<IEventProcessingService<T>>()
            .SingleOrDefault();
}