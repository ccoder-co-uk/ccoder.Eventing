using EventLibrary.Brokers;
using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Services.Processings;

public class EventProcessingService<T> : IEventProcessingService<T>
{
    private const string Guest = "Guest";

    private readonly IEventService<EventMessage<T>> eventService;
    private readonly IServiceProviderBroker serviceProviderBroker;
    private readonly ILogger<EventProcessingService<T>> log;

    public EventProcessingService(
        IEventService<EventMessage<T>> eventService,
        IServiceProviderBroker serviceProviderBroker,
        ILogger<EventProcessingService<T>> log)
    {
        this.eventService = eventService;
        this.serviceProviderBroker = serviceProviderBroker;
        this.log = log;
    }

    public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler)
    {
        async ValueTask ForwardToHandler(IServiceProvider serviceProvider, EventMessage<T> message)
        {
            log.LogDebug(
                "Handling event for {UserId} raising {EventName} event.",
                message?.AuthInfo?.SSOUserId ?? Guest,
                name);

            if (message is null)
            {
                log.LogWarning("Handler was given null when raising {EventName} event.", name);
                return;
            }

            await handler(serviceProvider, message.Data);
        }

        eventService.ListenToEvent(name, ForwardToHandler);
    }

    public async ValueTask RaiseEventAsync(string name, EventMessage<T> data)
    {
        log.LogDebug(
            "User {UserId} raising {EventName} event.",
            data?.AuthInfo?.SSOUserId ?? Guest,
            name);

        using IServiceScope scope = serviceProviderBroker.GetScopeForEvent(data);

        await eventService.RaiseEventAsync(name, scope.ServiceProvider, data);
    }
}
