using Azure.Messaging.ServiceBus;

namespace EventLibrary.AzureServiceBus.Brokers;

internal interface IServiceBusBroker
{
    ServiceBusProcessor CreateProcessor(string name);
    ValueTask StartProcessorAsync(string name);
    ValueTask SendMessageAsync(string name, ServiceBusMessage message);
}
