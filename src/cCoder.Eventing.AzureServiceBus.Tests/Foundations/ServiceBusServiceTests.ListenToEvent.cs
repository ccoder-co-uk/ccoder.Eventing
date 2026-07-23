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
        string inputName = "event-name";
        Mock<ServiceBusProcessor> serviceBusProcessorMock = new();

        serviceBusBrokerMock
            .Setup(broker => broker.CreateProcessor(inputName))
            .Returns(value:serviceBusProcessorMock.Object);

        serviceBusBrokerMock
            .Setup(broker => broker.StartProcessorAsync(inputName))
            .Returns(value:ValueTask.CompletedTask);

        serviceBusService.ListenToEvent<FakeObject>(
name:            inputName,
handler:            (_, _) => ValueTask.CompletedTask);

        serviceBusBrokerMock.Verify(
expression:            broker => broker.CreateProcessor(inputName),
times:            Times.Once);

        serviceBusBrokerMock.Verify(
expression:            broker => broker.StartProcessorAsync(inputName),
times:            Times.Once);
    }
}