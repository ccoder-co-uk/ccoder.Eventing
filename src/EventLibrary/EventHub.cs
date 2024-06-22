using EventLibrary.Brokers.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Services.Processing;
using EventLibrary.Services.Processing.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventLibrary
{
    /// <summary>
    /// 
    /// </summary>
    public class EventHub : IEventHub
    {
        readonly List<object> services = new();
        readonly IServiceProviderBroker serviceProviderBroker;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceProviderBroker"></param>
        public EventHub(IServiceProviderBroker serviceProviderBroker) =>
            this.serviceProviderBroker = serviceProviderBroker;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="handler"></param>
        public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler)
        {
            var service = GetEventService<T>();

            if (service is null)
            {
                services.Add(new EventProcessingService<T>(serviceProviderBroker));
                service = GetEventService<T>();
            }

            service.ListenToEvent(name, handler);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message)
        {
            ValidateRequest(name, message);

            var service = GetEventService<T>();

            if (service is not null)
                await service.RaiseEventAsync(name, message);
            else
                serviceProviderBroker.GetService<ILogger<EventHub>>()
                    .LogWarning("{Name} event was raised, but no handler was configured for it", name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="messages"></param>
        /// <returns></returns>
        public async ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages)
        {
            foreach (var message in messages)
                ValidateRequest(name, message);

            foreach (var message in messages)
                await RaiseEventAsync(name, message);
        }

        static void ValidateRequest<T>(string name, EventMessage<T> message)
        {
            if (name is null)
                throw new InvalidOperationException("You must provide an event name when raising events.");

            if (message is null)
                throw new InvalidOperationException("You must provide a message when raising events.");

            if (message.Data is null)
                throw new InvalidOperationException("You must provide some message data when raising events.");

            if (message.AuthInfo is null)
                throw new InvalidOperationException("You must provide some message auth information when raising events.");
        }

        IEventProcessingService<T> GetEventService<T>() => 
            services.Find(s => s.GetType().GenericTypeArguments[0] == typeof(T))
                    as IEventProcessingService<T>;
    }
}