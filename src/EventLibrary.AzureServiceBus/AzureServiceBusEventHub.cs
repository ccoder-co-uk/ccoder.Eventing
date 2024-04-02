using EventLibrary.AzureServiceBus.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Objects.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace EventLibrary.AzureServiceBus
{
    public class AzureServiceBusEventHub : IEventHub
    {
        readonly Func<IEventAuthInfo> getAuthInfo;
        readonly IAzureServiceBusCient serviceBusClient;

        public AzureServiceBusEventHub(Func<IEventAuthInfo> getAuthInfo, string serviceBusConnectionString, ILogger<AzureServiceBusEventHub> log)
        {
            this.getAuthInfo = getAuthInfo;
            serviceBusClient = new AzureServiceBusClient(new ServiceBusClient(serviceBusConnectionString), log);
        }

        // we currently don't use this as
        // we intend to use azure functions with SB triggers on them
        public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
            throw new NotImplementedException();

        public async ValueTask RaiseEventAsync<T>(string name, T data)
        {
            var eventMessage = new EventMessage<T>
            {
                AuthInfo = getAuthInfo(),
                Data = data
            };

            await serviceBusClient.RaiseEventAsync<T>(name, eventMessage);
        }

        public async ValueTask RaiseEventAsync<T>(string name, EventMessage<T> data) =>
            await serviceBusClient.RaiseEventAsync<T>(name, data);

        public async ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages) =>
            await serviceBusClient.RaiseEventsAsync(name, messages);
    }
}