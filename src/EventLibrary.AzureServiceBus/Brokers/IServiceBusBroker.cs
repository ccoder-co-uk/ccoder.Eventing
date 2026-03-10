using Azure.Messaging.ServiceBus;

namespace EventLibrary.AzureServiceBus.Brokers.Interfaces;

public interface IServiceBusBroker
{
    ValueTask SendMessageAsync(string name, ServiceBusMessage message);
}
