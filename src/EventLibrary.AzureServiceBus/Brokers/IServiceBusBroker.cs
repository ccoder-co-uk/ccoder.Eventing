using Azure.Messaging.ServiceBus;

namespace EventLibrary.AzureServiceBus.Brokers;

public interface IServiceBusBroker
{
    ValueTask SendMessageAsync(string name, ServiceBusMessage message);
}
