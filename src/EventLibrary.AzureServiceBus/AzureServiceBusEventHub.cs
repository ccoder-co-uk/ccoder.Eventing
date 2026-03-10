using EventLibrary.AzureServiceBus.Services.Processings;
using EventLibrary.Models;

namespace EventLibrary.AzureServiceBus;

public class AzureServiceBusEventHub : IAzureServiceBusEventHub
{
    private readonly IServiceBusProcessingService serviceBusProcessingService;

    public AzureServiceBusEventHub(IServiceBusProcessingService serviceBusProcessingService) =>
        this.serviceBusProcessingService = serviceBusProcessingService;

    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
        serviceBusProcessingService.ListenToEvent(name, handler);

    public ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        serviceBusProcessingService.RaiseEventAsync(name, message);

    public ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages) =>
        serviceBusProcessingService.RaiseEventsAsync(name, messages);
}
