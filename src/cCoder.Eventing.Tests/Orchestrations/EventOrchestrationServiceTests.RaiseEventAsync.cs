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

        // When

        await eventOrchestrationService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        eventProviderServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, message:inputMessage),
times: Times.Once);

        eventServiceProviderServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, message:inputMessage),
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

        eventServiceProviderServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:It.IsAny<string>(), message:It.IsAny<EventMessage<FakeObject>>()),
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

        eventProviderServiceMock
            .Setup(expression:service => service.RaiseEventsAsync(name:inputName, messages:inputMessages))
            .ReturnsAsync(value:false);

        // When

        await eventOrchestrationService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        eventProviderServiceMock.Verify(
expression: service => service.RaiseEventsAsync(name:inputName, messages:inputMessages),
times: Times.Once);

        eventServiceProviderServiceMock.Verify(
expression: service => service.RaiseEventsAsync(name:inputName, messages:inputMessages),
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

        eventProviderServiceMock
            .Setup(expression:service => service.RaiseEventsAsync(name:inputName, messages:inputMessages))
            .ReturnsAsync(value:true);

        // When

        await eventOrchestrationService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        eventServiceProviderServiceMock.Verify(
expression: service => service.RaiseEventsAsync(name:It.IsAny<string>(), messages:It.IsAny<EventMessage<FakeObject>[]>()),
times: Times.Never);
    }
}