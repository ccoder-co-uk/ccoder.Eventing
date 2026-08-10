// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Brokers.Loggings;

namespace cCoder.Eventing.Services.Processings;

internal sealed partial class EventProcessingService<T>(
        IEventService<T> eventService,
        ILoggingBroker log)
            : IEventProcessingService<T>
{
    public void ListenToEvent(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);
            log.LogDebug(message:"Listening to event {EventName}", args:name);

            async ValueTask ForwardToHandler(IServiceProvider serviceProvider, T message)
            {
                if(message is not null)
                {
                    await handler(arg1:serviceProvider, arg2:message);
                }
            }

            eventService.ListenToEvent(name:name, handler:ForwardToHandler);
        });

    public ValueTask RaiseEventAsync(
        string name,
        EventMessage<T> message) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, message]);

            log.LogDebug(
                message: "Creating event scope for user {UserId}",
                args: message?
                    .AuthInfo?
                    .SSOUserId);

            await eventService.RaiseEventAsync(name:name, message:message);
        });
}