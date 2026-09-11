// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;

namespace cCoder.Eventing.Services.Orchestrations;

internal sealed partial class EventOrchestrationService(
    IEventProviderService eventProviderService,
    IEventServiceProviderService eventServiceProviderService) 
        : IEventOrchestrationService
{
    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);
            eventServiceProviderService.ListenToEvent(name:name, handler:handler);
        });

    public void ListenToEvent<TMessage, THandlingService>(
        string name,
        Func<THandlingService, TMessage, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            eventServiceProviderService.ListenToEvent<TMessage, THandlingService>(
                name: name,
                handler: handler);
        });

    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, message]);

            bool handled = await eventProviderService.RaiseEventAsync(name:name, message:message);

            if (!handled)
            {
                await eventServiceProviderService.RaiseEventAsync(name:name, message:message);
            }
        });

    public ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, messages]);

            bool handled = await eventProviderService.RaiseEventsAsync(name:name, messages:messages);

            if (!handled)
            {
                await eventServiceProviderService.RaiseEventsAsync(name:name, messages:messages);
            }
        });
}