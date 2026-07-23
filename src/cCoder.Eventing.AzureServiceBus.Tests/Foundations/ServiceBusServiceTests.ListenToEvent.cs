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
            .Returns(serviceBusProcessorMock.Object);

        serviceBusBrokerMock
            .Setup(broker => broker.StartProcessorAsync(inputName))
            .Returns(ValueTask.CompletedTask);

        serviceBusService.ListenToEvent<FakeObject>(
            inputName,
            (_, _) => ValueTask.CompletedTask);

        serviceBusBrokerMock.Verify(
            broker => broker.CreateProcessor(inputName),
            Times.Once);

        serviceBusBrokerMock.Verify(
            broker => broker.StartProcessorAsync(inputName),
            Times.Once);
    }
}