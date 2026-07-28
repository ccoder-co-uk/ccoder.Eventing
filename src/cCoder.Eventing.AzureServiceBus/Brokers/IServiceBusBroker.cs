// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal interface IServiceBusBroker
{
    ServiceBusSender CreateSender(string name);
    ServiceBusProcessor CreateProcessor(string name);
    Task SendMessageAsync(
        ServiceBusSender sender,
        ServiceBusMessage message);
    Task StartProcessorAsync(ServiceBusProcessor processor);
}