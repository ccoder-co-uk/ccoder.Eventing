// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Brokers.Loggings;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Processings;

namespace cCoder.Eventing.Services.Orchestrations;

internal sealed partial class EventOrchestrationService(
    IEventProviderProcessingService eventProviderService,
    IBulkEventProviderProcessingService bulkEventProviderService,
    IServiceProviderBroker serviceProviderBroker,
    ILoggingBroker log)
        : IEventOrchestrationService
{
    private readonly List<object> services = [];

    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            GetOrCreateEventProcessingService<T>()
                .ListenToEvent(name: name, handler: handler);
        });

    public void ListenToEvent<TMessage, THandlingService>(
        string name,
        Func<THandlingService, TMessage, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            Func<IServiceProvider, TMessage, ValueTask> internalHandler =
                async (serviceProvider, message) =>
                {
                    THandlingService handlingService =
                        serviceProviderBroker.GetRequiredService<THandlingService>(
                            serviceProvider: serviceProvider);

                    await handler(arg1: handlingService, arg2: message);
                };

            GetOrCreateEventProcessingService<TMessage>()
                .ListenToEvent(name: name, handler: internalHandler);
        });

    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, message]);

            bool handled = await eventProviderService
                .RaiseEventAsync(name: name, message: message);

            if (!handled)
            {
                await RaiseEventInternallyAsync(name: name, message: message);
            }
        });

    public ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, messages]);

            bool handled = await bulkEventProviderService
                .RaiseEventsAsync(name: name, messages: messages);

            if (!handled)
            {
                foreach (EventMessage<T> message in messages)
                {
                    await RaiseEventInternallyAsync(name: name, message: message);
                }
            }
        });

    private async ValueTask RaiseEventInternallyAsync<T>(
        string name,
        EventMessage<T> message)
    {
        ValidateRequest(name: name, message: message);

        IEventProcessingService<T> service = GetEventProcessingService<T>();

        if (service is null)
        {
            log.LogWarning(
                message: "{Name} event was raised, but no handler was configured for it",
                args: name);

            return;
        }

        await service.RaiseEventAsync(name: name, data: message);
    }

    private static void ValidateRequest<T>(
        string name,
        EventMessage<T> message)
    {
        if (name is null)
        {
            throw new InvalidOperationException(
                message: "You must provide an event name when raising events.");
        }

        if (message is null)
        {
            throw new InvalidOperationException(
                message: "You must provide a message when raising events.");
        }

        if (message.Data is null)
        {
            throw new InvalidOperationException(
                message: "You must provide some message data when raising events.");
        }

        if (message.AuthInfo is null)
        {
            throw new InvalidOperationException(
                message: "You must provide some message auth information when raising events.");
        }
    }

    private IEventProcessingService<T> GetOrCreateEventProcessingService<T>()
    {
        IEventProcessingService<T> service = GetEventProcessingService<T>();

        if (service is null)
        {
            service = serviceProviderBroker.GetService<IEventProcessingService<T>>();
            services.Add(item: service);
        }

        return service;
    }

    private IEventProcessingService<T> GetEventProcessingService<T>() =>
        services
            .OfType<IEventProcessingService<T>>()
            .SingleOrDefault();
}