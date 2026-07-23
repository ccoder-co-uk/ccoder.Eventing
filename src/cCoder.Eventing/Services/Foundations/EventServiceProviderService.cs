// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Processings;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Services.Foundations;

internal class EventServiceProviderService : IEventServiceProviderService
{
    private readonly List<object> services = [];
    private readonly IServiceProviderBroker serviceProviderBroker;
    private readonly ILogger<EventServiceProviderService> log;

    public EventServiceProviderService(
        IServiceProviderBroker serviceProviderBroker,
        ILogger<EventServiceProviderService> log)
    {
        this.serviceProviderBroker = serviceProviderBroker;
        this.log = log;
    }

    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler)
    {
        try
        {
            IEventProcessingService<T> typedEventService = GetEventService<T>();

            if (typedEventService is null)
            {
                typedEventService = serviceProviderBroker.GetService<IEventProcessingService<T>>();
                services.Add(typedEventService);
            }

            typedEventService.ListenToEvent(name, handler);
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst listening to {Name} event\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }

    public async ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message)
    {
        try
        {
            ValidateRequest(name, message);

            IEventProcessingService<T> service = GetEventService<T>();

            if (service is null)
            {
                log.LogWarning("{Name} event was raised, but no handler was configured for it", name);
                return;
            }

            await service.RaiseEventAsync(name, message);
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst raising {Name} event\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }

    public async ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages)
    {
        try
        {
            foreach (EventMessage<T> message in messages)
                await RaiseEventAsync(name, message);
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst raising {Name} events\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw;
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