// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal sealed class ServiceBusBroker(
    ServiceBusClient serviceBusClient,
    AzureServiceBusEventingConfiguration configuration) : IServiceBusBroker
{
    public ServiceBusSender CreateSender(string name) =>
        serviceBusClient.CreateSender(queueOrTopicName: name);

    public ServiceBusProcessor CreateProcessor(string name) =>
        serviceBusClient.CreateProcessor(
            queueName: name,
            options: new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = Math.Max(
                    val1: 1,
                    val2: configuration.MaxConcurrency)
            });

    public Task SendMessageAsync(
        ServiceBusSender sender,
        ServiceBusMessage message) =>
        sender.SendMessageAsync(message: message);

    public Task StartProcessorAsync(ServiceBusProcessor processor) =>
        processor.StartProcessingAsync();
}