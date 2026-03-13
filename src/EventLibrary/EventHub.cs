using EventLibrary.Models;
using EventLibrary.Services.Foundations;

namespace EventLibrary;

public class EventHub : IEventHub
{
    private readonly IEventServiceProviderService eventServiceProviderService;

    public EventHub(IEventServiceProviderService eventServiceProviderService) =>
        this.eventServiceProviderService = eventServiceProviderService;

    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
        eventServiceProviderService.ListenToEvent(name, handler);

    public ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        eventServiceProviderService.RaiseEventAsync(name, message);

    public ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages) =>
        eventServiceProviderService.RaiseEventsAsync(name, messages);
}
