using EventLibrary.AzureServiceBus.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Objects.Interfaces;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Logging;

namespace EventLibrary.AzureServiceBus
{
    /// <summary>
    /// 
    /// </summary>
    public class AzureServiceBusEventHub : IEventHub
    {
        readonly Func<IEventAuthInfo> getAuthInfo;
        readonly IAzureServiceBusCient serviceBusClient;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="getAuthInfo"></param>
        /// <param name="serviceBusConnectionString"></param>
        /// <param name="log"></param>
        public AzureServiceBusEventHub(Func<IEventAuthInfo> getAuthInfo, string serviceBusConnectionString, ILogger<AzureServiceBusEventHub> log)
        {
            this.getAuthInfo = getAuthInfo;
            serviceBusClient = new AzureServiceBusClient(new ServiceBusClient(serviceBusConnectionString), log);
        }

        // we currently don't use this as
        // we intend to use azure functions with SB triggers on them
        public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
            throw new NotImplementedException();

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async ValueTask RaiseEventAsync<T>(string name, T data)
        {
            var eventMessage = new EventMessage<T>
            {
                AuthInfo = getAuthInfo(),
                Data = data
            };

            ValidateRequest(name, eventMessage);

            await serviceBusClient.RaiseEventAsync<T>(name, eventMessage);
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
            await serviceBusClient.RaiseEventAsync<T>(name, message);
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

            await serviceBusClient.RaiseEventsAsync(name, messages);
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
    }
}