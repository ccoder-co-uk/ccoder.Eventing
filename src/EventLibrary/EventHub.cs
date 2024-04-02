using EventLibrary.Brokers.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Services.Processing;
using EventLibrary.Services.Processing.Interfaces;
using Microsoft.Extensions.Logging;

namespace EventLibrary
{
    public class EventHub : IEventHub
    {
        readonly List<object> services = new();
        readonly IServiceProviderBroker serviceProviderBroker;

        public EventHub(IServiceProviderBroker serviceProviderBroker) =>
            this.serviceProviderBroker = serviceProviderBroker;

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

        public async ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message)
        {
            var service = GetEventService<T>();

            if (service is not null)
                await service.RaiseEventAsync(name, message);
            else
                serviceProviderBroker.GetService<ILogger<EventHub>>()
                    .LogWarning($"{name} event was raised, but no handler was configured for it");
        }

        public async ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages)
        {
            foreach(var message in messages)
                await RaiseEventAsync(name, message);
        }

        IEventProcessingService<T> GetEventService<T>() => 
            services.FirstOrDefault(s => s.GetType().GenericTypeArguments[0] == typeof(T))
                    as IEventProcessingService<T>;
    }
}