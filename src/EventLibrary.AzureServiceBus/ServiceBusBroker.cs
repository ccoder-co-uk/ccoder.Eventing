using Azure.Messaging.ServiceBus;
using EventLibrary.AzureServiceBus.Interfaces;
using EventLibrary.Objects;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace EventLibrary.AzureServiceBus
{
    public class AzureServiceBusClient : IAzureServiceBusCient
    {
        readonly ServiceBusClient serviceBusClient;
        private readonly ILogger log;
        readonly IDictionary<string, ServiceBusSender> senders;

        public AzureServiceBusClient(ServiceBusClient serviceBusClient, ILogger log)
        {
            this.serviceBusClient = serviceBusClient;
            this.log = log;

            senders = new Dictionary<string, ServiceBusSender>();
        }

        public async ValueTask RaiseEventAsync<T>(string name, EventMessage<T> eventMessage)
        {
            try
            {
                var message = new ServiceBusMessage()
                {
                    Body = new BinaryData(eventMessage),
                    MessageId = $"{eventMessage.AuthInfo.SSOUserId}_{typeof(T).Name}_{Guid.NewGuid()}"
                };

                var sender = senders.ContainsKey(name)
                    ? senders[name]
                    : CreateNewSender(name);

                await sender.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                log.LogError($"Exception thrown whilst raising {name} event", ex);
                log.LogError(ex.Message);
                log.LogError(ex.StackTrace);

                if (ex.InnerException is not null)
                {
                    log.LogError("Inner Exception: ", ex.InnerException);
                    log.LogError(ex.InnerException.Message);
                    log.LogError(ex.InnerException.StackTrace);
                }

                throw;
            }
        }

        public async ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] eventMessages)
        {
            try
            {
                int maxSize = 230000;
                int currentSize = 0;
                int currentMessageLength = 0;
                List<ServiceBusMessage> chunk = [];
                List<EventMessage<T>> failed = [];

                var sender = senders.ContainsKey(name)
                    ? senders[name]
                    : CreateNewSender(name);

                foreach(var eventMessage in eventMessages)
                {
                    var json = JsonConvert.SerializeObject(eventMessage, Formatting.Indented);
                    Byte[] bytes = Encoding.UTF8.GetBytes(json);
                    var message = new ServiceBusMessage()
                    {
                        Body = new BinaryData(bytes),
                        MessageId = $"{eventMessage.AuthInfo.SSOUserId}_{typeof(T).Name}_{Guid.NewGuid()}"
                    };

                    currentMessageLength = message.MessageId.Length + bytes.Length + 100;

                    if (currentSize + currentMessageLength > maxSize)
                    {
                        if (chunk.Count == 1)
                        {
                            log.LogError(json);
                            failed.Add(eventMessage);
                        }
                        else
                        {
                            await sender.SendMessagesAsync(chunk);
                            chunk.Clear();
                            chunk.Add(message);
                            currentSize = currentMessageLength;
                        }
                    }
                    else
                    {
                        currentSize += currentMessageLength;
                        chunk.Add(message);
                    }
                }

                if (chunk.Any())
                    await sender.SendMessagesAsync(chunk);

                if (failed.Any()) {
                    var ex = new InvalidOperationException("Failed to send events to ServiceBus.");
                    ex.Data.Add("failures", failed);
                    throw ex;
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Exception thrown whilst raising {name} event", ex);
                log.LogError(ex.Message);
                log.LogError(ex.StackTrace);

                if (ex.InnerException is not null)
                {
                    log.LogError("Inner Exception: ", ex.InnerException);
                    log.LogError(ex.InnerException.Message);
                    log.LogError(ex.InnerException.StackTrace);
                }

                throw;
            }
        }

        private ServiceBusSender CreateNewSender(string name)
        {
            lock(senders)
            {
                senders[name] = serviceBusClient.CreateSender(name);
                return senders[name];
            }
        }

        ~AzureServiceBusClient() 
        {
            foreach(var sender in senders)
                sender.Value.DisposeAsync().AsTask().Wait();
        }
    }
}