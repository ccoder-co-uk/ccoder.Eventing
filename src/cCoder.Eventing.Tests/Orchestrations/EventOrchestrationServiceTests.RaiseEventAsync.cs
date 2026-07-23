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
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        eventProviderServiceMock
            .Setup(service => service.RaiseEventAsync(inputName, inputMessage))
            .ReturnsAsync(value:false);

        await eventOrchestrationService.RaiseEventAsync(name:inputName, message:inputMessage);

        eventProviderServiceMock.Verify(
expression:            service => service.RaiseEventAsync(inputName, inputMessage),
times:            Times.Once);

        eventServiceProviderServiceMock.Verify(
expression:            service => service.RaiseEventAsync(inputName, inputMessage),
times:            Times.Once);
    }

    [Fact]
    public async Task ShouldNotRaiseEventAsyncInternallyWhenExternalProviderHandlesIt()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        eventProviderServiceMock
            .Setup(service => service.RaiseEventAsync(inputName, inputMessage))
            .ReturnsAsync(value:true);

        await eventOrchestrationService.RaiseEventAsync(name:inputName, message:inputMessage);

        eventServiceProviderServiceMock.Verify(
expression:            service => service.RaiseEventAsync(It.IsAny<string>(), It.IsAny<EventMessage<FakeObject>>()),
times:            Times.Never);
    }

    [Fact]
    public async Task ShouldRaiseEventsAsync()
    {
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
            .Setup(service => service.RaiseEventsAsync(inputName, inputMessages))
            .ReturnsAsync(value:false);

        await eventOrchestrationService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        eventProviderServiceMock.Verify(
expression:            service => service.RaiseEventsAsync(inputName, inputMessages),
times:            Times.Once);

        eventServiceProviderServiceMock.Verify(
expression:            service => service.RaiseEventsAsync(inputName, inputMessages),
times:            Times.Once);
    }

    [Fact]
    public async Task ShouldNotRaiseEventsAsyncInternallyWhenExternalBulkProviderHandlesIt()
    {
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
            .Setup(service => service.RaiseEventsAsync(inputName, inputMessages))
            .ReturnsAsync(value:true);

        await eventOrchestrationService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        eventServiceProviderServiceMock.Verify(
expression:            service => service.RaiseEventsAsync(It.IsAny<string>(), It.IsAny<EventMessage<FakeObject>[]>()),
times:            Times.Never);
    }
}