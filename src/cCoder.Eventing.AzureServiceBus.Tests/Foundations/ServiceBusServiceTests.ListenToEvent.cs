// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        // Given

        string inputName = "event-name";
        Mock<ServiceBusProcessor> serviceBusProcessorMock = new();

        serviceBusBrokerMock
            .Setup(expression:broker => broker.CreateProcessor(name:inputName))
            .Returns(value:serviceBusProcessorMock.Object);

        serviceBusBrokerMock
            .Setup(expression:broker => broker.StartProcessorAsync(
                processor: serviceBusProcessorMock.Object))
            .Returns(value: Task.CompletedTask);

        // When

        serviceBusService.ListenToEvent<FakeObject>(
name: inputName,
handler: (_, _) => ValueTask.CompletedTask);

        // Then

        serviceBusBrokerMock.Verify(
expression: broker => broker.CreateProcessor(name:inputName),
times: Times.Once);

        serviceBusBrokerMock.Verify(
expression: broker => broker.StartProcessorAsync(
    processor: serviceBusProcessorMock.Object),
times: Times.Once);
    }
}