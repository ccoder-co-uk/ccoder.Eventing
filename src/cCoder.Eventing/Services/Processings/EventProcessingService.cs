// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Services.Processings;

internal class EventProcessingService<T>(
        IEventService<T> eventService,
        ILogger<EventProcessingService<T>> log) 
            : IEventProcessingService<T>
{
    public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler)
    {
        log.LogDebug("Listening to event {EventName}", name);

        async ValueTask ForwardToHandler(IServiceProvider serviceProvider, T message)
        {
            if(message is not null)
                await handler(serviceProvider, message);
        }

        eventService.ListenToEvent(name, ForwardToHandler);
    }

    public async ValueTask RaiseEventAsync(string name, EventMessage<T> message)
    {
        log.LogDebug("Creating event scope for user {UserId}", message?.AuthInfo?.SSOUserId);
        await eventService.RaiseEventAsync(name, message);
    }
}