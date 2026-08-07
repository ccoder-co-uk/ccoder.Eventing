// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Exposures;

public partial class AzureServiceBusEventHubTests
{
    [Fact]
    public async Task ShouldForwardEveryExposureOperation()
    {
        // Given

        const string eventName = "test-event";
        ServiceBusEventMessage<FakeObject> message = new();
        ServiceBusEventMessage<FakeObject>[] messages = [message];

        Func<IServiceProvider, FakeObject, ValueTask> handler =
            (_, _) => ValueTask.CompletedTask;

        Mock<IServiceBusProcessingService> processingService = new();

        AzureServiceBusEventHub eventHub = new(
            serviceBusProcessingService: processingService.Object);

        // When

        eventHub.ListenToEvent(name: eventName, handler: handler);
        await eventHub.RaiseEventAsync(name: eventName, message: message);
        await eventHub.RaiseEventsAsync(name: eventName, messages: messages);

        // Then

        processingService.Verify(
            expression: service => service.ListenToEvent(
                name: eventName,
                handler: handler),
            times: Times.Once);

        processingService.Verify(
            expression: service => service.RaiseEventAsync(
                name: eventName,
                message: message),
            times: Times.Once);

        processingService.Verify(
            expression: service => service.RaiseEventsAsync(
                name: eventName,
                messages: messages),
            times: Times.Once);
    }
}