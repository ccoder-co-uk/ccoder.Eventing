using EventLibrary.Brokers.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Objects.Interfaces;
using EventLibrary.Services.Foundation;
using EventLibrary.Services.Foundation.Interfaces;
using EventLibrary.Services.Processing.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Services.Processing
{
    public class EventProcessingService<T> : IEventProcessingService<T>
    {
        IEventService<EventMessage<T>> eventService;
        private readonly IServiceProviderBroker serviceProviderBroker;
        readonly ILogger log;

        public EventProcessingService(IServiceProviderBroker serviceProviderBroker)
        {
            this.serviceProviderBroker = serviceProviderBroker;
            log = serviceProviderBroker.GetService<ILogger<EventProcessingService<T>>>();
        }

        public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler)
        {
            async ValueTask func(IServiceProvider serviceProvider, EventMessage<T> message)
            {
                log.LogDebug($"Handling event {name} raised by user {message.AuthInfo.SSOUserId}.");

                var serviceProviderBroker = serviceProvider.GetService<IServiceProviderBroker>();
                await handler(serviceProvider, message.Data);
            }

            eventService ??= new EventService<EventMessage<T>>(
                new EventBroker<EventMessage<T>>(),
                serviceProviderBroker.GetService<ILogger<EventService<EventMessage<T>>>>()
            );

            eventService.ListenToEvent(name, func);
        }

        public async ValueTask RaiseEventAsync(string name, EventMessage<T> data)
        {
            log.LogDebug($"User {data.AuthInfo.SSOUserId} raising {name} event.");
            using var scope = serviceProviderBroker.GetScopeForEvent(data);

            if (eventService is not null)
                await eventService.RaiseEventAsync(name, scope.ServiceProvider, data);
        }
    }
}