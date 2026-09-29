// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Processings;
using Moq;

namespace cCoder.Eventing.AzureServiceBus.AcceptanceTests.Hubs;

public sealed partial class AzureServiceBusExposureAcceptanceTests
{
    [Fact]
    public async Task ShouldForwardEveryAzureServiceBusHubOperation()
    {
        // Given

        const string eventName = "test-event";
        ServiceBusEventMessage<string> message = new();
        ServiceBusEventMessage<string>[] messages = [message];
        Func<IServiceProvider, string, ValueTask> handler = (_, _) => ValueTask.CompletedTask;
        Mock<IServiceBusProcessingService> processingService = new();

        AzureServiceBusEventHub eventHub = new(
            serviceBusProcessingService: processingService.Object);

        // When

        eventHub.ListenToEvent(name: eventName, handler: handler);
        await eventHub.RaiseEventAsync(name: eventName, message: message);
        await eventHub.RaiseEventsAsync(name: eventName, messages: messages);

        // Then

        processingService.Verify(
            expression: service => service.ListenToEvent(name: eventName, handler: handler),
            times: Times.Once);

        processingService.Verify(
            expression: service => service.RaiseEventAsync(name: eventName, message: message),
            times: Times.Once);

        processingService.Verify(
            expression: service => service.RaiseEventsAsync(name: eventName, messages: messages),
            times: Times.Once);
    }
}