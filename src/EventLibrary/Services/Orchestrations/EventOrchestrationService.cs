using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.Services.Orchestrations;

internal class EventOrchestrationService(
    IEventServiceProviderService eventServiceProviderService) 
        : IEventOrchestrationService
{
    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
        eventServiceProviderService.ListenToEvent(name, handler);

    public void ListenToEvent<TMessage, THandlingService>(
        string name,
        Func<THandlingService, TMessage, ValueTask> handler)
    {
        Func<IServiceProvider, TMessage, ValueTask> internalHandler =
            async (serviceProvider, message) =>
            {
                THandlingService handlingService =
                    serviceProvider.GetRequiredService<THandlingService>();

                await handler(handlingService, message);
            };

        eventServiceProviderService
            .ListenToEvent(name, internalHandler);
    }

    public ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        eventServiceProviderService.RaiseEventAsync(name, message);

    public ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages) =>
        eventServiceProviderService.RaiseEventsAsync(name, messages);
}
