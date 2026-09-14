// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Orchestrations;

public partial class EventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsync()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        eventProviderServiceMock
            .Setup(expression:service => service.RaiseEventAsync(name:inputName, message:inputMessage))
            .ReturnsAsync(value:false);

        eventOrchestrationService.ListenToEvent<FakeObject>(
            name: inputName,
            handler: (_, _) => ValueTask.CompletedTask);

        // When

        await eventOrchestrationService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        eventProviderServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, message:inputMessage),
times: Times.Once);

        eventProcessingServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, data:inputMessage),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldNotRaiseEventAsyncInternallyWhenExternalProviderHandlesIt()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        eventProviderServiceMock
            .Setup(expression:service => service.RaiseEventAsync(name:inputName, message:inputMessage))
            .ReturnsAsync(value:true);

        // When

        await eventOrchestrationService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        eventProcessingServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:It.IsAny<string>(), data:It.IsAny<EventMessage<FakeObject>>()),
times: Times.Never);
    }

    [Fact]
    public async Task ShouldRaiseEventsAsync()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        bulkEventProviderServiceMock
            .Setup(expression:service => service.RaiseEventsAsync(name:inputName, messages:inputMessages))
            .ReturnsAsync(value:false);

        eventOrchestrationService.ListenToEvent<FakeObject>(
            name: inputName,
            handler: (_, _) => ValueTask.CompletedTask);

        // When

        await eventOrchestrationService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        bulkEventProviderServiceMock.Verify(
expression: service => service.RaiseEventsAsync(name:inputName, messages:inputMessages),
times: Times.Once);

        eventProcessingServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, data:inputMessages[0]),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldNotRaiseEventsAsyncInternallyWhenExternalBulkProviderHandlesIt()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        bulkEventProviderServiceMock
            .Setup(expression:service => service.RaiseEventsAsync(name:inputName, messages:inputMessages))
            .ReturnsAsync(value:true);

        // When

        await eventOrchestrationService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        eventProcessingServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:It.IsAny<string>(), data:It.IsAny<EventMessage<FakeObject>>()),
times: Times.Never);
    }
}