// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Orchestrations;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Exposures;

public partial class EventHubTests
{
    private readonly Mock<IEventOrchestrationService> eventOrchestrationServiceMock = new();

    [Fact]
    public void ShouldForwardEventListener()
    {
        // Given

        const string eventName = "test-event";
        Func<IServiceProvider, string, ValueTask> handler = (_, _) => ValueTask.CompletedTask;
        var eventHub = new EventHub(eventOrchestrationService:this.eventOrchestrationServiceMock.Object);

        // When

        eventHub.ListenToEvent(name:eventName, handler:handler);

        // Then

        this.eventOrchestrationServiceMock.Verify(
            expression:service => service.ListenToEvent(name:eventName, handler:handler),
            times:Times.Once);
    }

    [Fact]
    public void ShouldForwardServiceEventListener()
    {
        // Given

        const string eventName = "test-event";
        Func<TestEventHandler, string, ValueTask> handler = (_, _) => ValueTask.CompletedTask;
        var eventHub = new EventHub(eventOrchestrationService:this.eventOrchestrationServiceMock.Object);

        // When

        eventHub.ListenToEvent<string, TestEventHandler>(name:eventName, handler:handler);

        // Then

        this.eventOrchestrationServiceMock.Verify(
            expression:service => service.ListenToEvent(name:eventName, handler:handler),
            times:Times.Once);
    }

    [Fact]
    public async Task ShouldForwardEventAsync()
    {
        // Given

        const string eventName = "test-event";
        var message = new EventMessage<string>();
        var eventHub = new EventHub(eventOrchestrationService:this.eventOrchestrationServiceMock.Object);

        // When

        await eventHub.RaiseEventAsync(name:eventName, message:message);

        // Then

        this.eventOrchestrationServiceMock.Verify(
            expression:service => service.RaiseEventAsync(name:eventName, message:message),
            times:Times.Once);
    }

    [Fact]
    public async Task ShouldForwardEventsAsync()
    {
        // Given

        const string eventName = "test-event";
        EventMessage<string>[] messages = [new EventMessage<string>()];
        var eventHub = new EventHub(eventOrchestrationService:this.eventOrchestrationServiceMock.Object);

        // When

        await eventHub.RaiseEventsAsync(name:eventName, messages:messages);

        // Then

        this.eventOrchestrationServiceMock.Verify(
            expression:service => service.RaiseEventsAsync(name:eventName, messages:messages),
            times:Times.Once);
    }

    private sealed class TestEventHandler;
}