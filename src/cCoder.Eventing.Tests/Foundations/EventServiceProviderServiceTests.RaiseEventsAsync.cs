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
            .Returns(eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(inputName, (_, _) => ValueTask.CompletedTask);

        await eventServiceProviderService.RaiseEventsAsync(inputName, inputMessages);

        eventProcessingServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessages[0]),
            Times.Once);

        eventProcessingServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessages[1]),
            Times.Once);
    }
}