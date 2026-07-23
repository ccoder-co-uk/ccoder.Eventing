// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
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
            },
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(name:inputName, handler:(_, _) => ValueTask.CompletedTask);

        await eventServiceProviderService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        eventProcessingServiceMock.Verify(
expression:            service => service.RaiseEventAsync(inputName, inputMessages[0]),
times:            Times.Once);

        eventProcessingServiceMock.Verify(
expression:            service => service.RaiseEventAsync(inputName, inputMessages[1]),
times:            Times.Once);
    }
}