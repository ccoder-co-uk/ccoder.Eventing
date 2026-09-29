// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Orchestrations;
using Moq;

namespace cCoder.Eventing.AcceptanceTests.Hubs;

public sealed partial class EventHubExposureAcceptanceTests
{
    [Fact]
    public async Task ShouldForwardEveryEventHubOperation()
    {
        // Given

        const string eventName = "test-event";
        Func<IServiceProvider, string, ValueTask> handler = (_, _) => ValueTask.CompletedTask;
        Func<TestEventHandler, string, ValueTask> serviceHandler = (_, _) => ValueTask.CompletedTask;
        EventMessage<string> message = new();
        EventMessage<string>[] messages = [message];
        Mock<IEventOrchestrationService> orchestrationService = new();
        EventHub eventHub = new(eventOrchestrationService: orchestrationService.Object);

        // When

        eventHub.ListenToEvent(name: eventName, handler: handler);
        eventHub.ListenToEvent<string, TestEventHandler>(name: eventName, handler: serviceHandler);
        await eventHub.RaiseEventAsync(name: eventName, message: message);
        await eventHub.RaiseEventsAsync(name: eventName, messages: messages);

        // Then

        orchestrationService.Verify(
            expression: service => service.ListenToEvent(name: eventName, handler: handler),
            times: Times.Once);

        orchestrationService.Verify(
            expression: service => service.ListenToEvent(name: eventName, handler: serviceHandler),
            times: Times.Once);

        orchestrationService.Verify(
            expression: service => service.RaiseEventAsync(name: eventName, message: message),
            times: Times.Once);

        orchestrationService.Verify(
            expression: service => service.RaiseEventsAsync(name: eventName, messages: messages),
            times: Times.Once);
    }

    private sealed class TestEventHandler;
}