using EventLibrary.Brokers.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Services.Foundation;
using EventLibrary.Services.Foundation.Interfaces;
using EventLibrary.Services.Processing.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Services.Processing
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EventProcessingService<T> : IEventProcessingService<T>
    {
        IEventService<EventMessage<T>> eventService;
        private readonly IServiceProviderBroker serviceProviderBroker;
        readonly ILogger log;

        const string guest = "Guest";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProviderBroker"></param>
        public EventProcessingService(IServiceProviderBroker serviceProviderBroker)
        {
            this.serviceProviderBroker = serviceProviderBroker;
            log = serviceProviderBroker.GetService<ILogger<EventProcessingService<T>>>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler)
        {
            async ValueTask func(IServiceProvider serviceProvider, EventMessage<T> message)
            {
                log.LogDebug("Handling event for {UserId} raising {EventName} event.", message?.AuthInfo?.SSOUserId ?? guest, name);

                if(message is not null)
                    await handler(serviceProvider, message.Data);
                else
                    log.LogWarning("Handler was given null when raising {EventName} event.", name);
            }

            eventService ??= new EventService<EventMessage<T>>(
                new EventBroker<EventMessage<T>>(),
                serviceProviderBroker.GetService<ILogger<EventService<EventMessage<T>>>>()
            );

            eventService.ListenToEvent(name, func);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async ValueTask RaiseEventAsync(string name, EventMessage<T> data)
        {
            log.LogDebug("User {UserId} raising {EventName} event.", data?.AuthInfo?.SSOUserId ?? guest, name);

            using var scope = serviceProviderBroker.GetScopeForEvent(data);

            if (eventService is not null)
                await eventService.RaiseEventAsync(name, scope.ServiceProvider, data);
        }
    }
}