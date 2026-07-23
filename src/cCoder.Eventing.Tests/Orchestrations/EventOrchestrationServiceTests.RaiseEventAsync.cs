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
            .ReturnsAsync(false);

        await eventOrchestrationService.RaiseEventAsync(inputName, inputMessage);

        eventProviderServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessage),
            Times.Once);

        eventServiceProviderServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessage),
            Times.Once);
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
            .ReturnsAsync(true);

        await eventOrchestrationService.RaiseEventAsync(inputName, inputMessage);

        eventServiceProviderServiceMock.Verify(
            service => service.RaiseEventAsync(It.IsAny<string>(), It.IsAny<EventMessage<FakeObject>>()),
            Times.Never);
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
            .ReturnsAsync(false);

        await eventOrchestrationService.RaiseEventsAsync(inputName, inputMessages);

        eventProviderServiceMock.Verify(
            service => service.RaiseEventsAsync(inputName, inputMessages),
            Times.Once);

        eventServiceProviderServiceMock.Verify(
            service => service.RaiseEventsAsync(inputName, inputMessages),
            Times.Once);
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
            .ReturnsAsync(true);

        await eventOrchestrationService.RaiseEventsAsync(inputName, inputMessages);

        eventServiceProviderServiceMock.Verify(
            service => service.RaiseEventsAsync(It.IsAny<string>(), It.IsAny<EventMessage<FakeObject>[]>()),
            Times.Never);
    }
}