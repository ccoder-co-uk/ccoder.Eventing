// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal interface IServiceBusBroker
{
    ServiceBusProcessor CreateProcessor(string name);
    ValueTask StartProcessorAsync(string name);
    ValueTask SendMessageAsync(string name, ServiceBusMessage message);
}