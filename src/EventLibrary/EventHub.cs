using EventLibrary.Brokers;
using EventLibrary.Models;
using EventLibrary.Services.Processings;
using Microsoft.Extensions.Logging;

namespace EventLibrary;

public class EventHub : IEventHub
{
    private readonly List<object> services = [];
    private readonly IServiceProviderBroker serviceProviderBroker;

    public EventHub(IServiceProviderBroker serviceProviderBroker) =>
        this.serviceProviderBroker = serviceProviderBroker;

    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler)
    {
        IEventProcessingService<T> service = GetEventService<T>();

        if (service is null)
        {
            service = serviceProviderBroker.GetService<IEventProcessingService<T>>();
            services.Add(service);
        }

        service.ListenToEvent(name, handler);
    }

    public async ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message)
    {
        ValidateRequest(name, message);

        IEventProcessingService<T> service = GetEventService<T>();

        if (service is null)
        {
            serviceProviderBroker
                .GetService<ILogger<EventHub>>()
                .LogWarning("{Name} event was raised, but no handler was configured for it", name);

            return;
        }

        await service.RaiseEventAsync(name, message);
    }

    public async ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages)
    {
        foreach (EventMessage<T> message in messages)
        {
            ValidateRequest(name, message);
            await RaiseEventAsync(name, message);
        }
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
        services.OfType<IEventProcessingService<T>>().SingleOrDefault();
}
