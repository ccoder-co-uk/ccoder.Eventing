// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    [Fact]
    public void ShouldRethrowOnListenToEventIfBrokerFails()
    {
        string inputName = "event-name";
        Exception innerException = new("Broker failure");

        serviceBusBrokerMock
            .Setup(broker => broker.CreateProcessor(inputName))
            .Throws(exception:innerException);

        Action listenToEventAction = () =>
            serviceBusService.ListenToEvent<FakeObject>(
name:                inputName,
handler:                (_, _) => ValueTask.CompletedTask);

        Exception actualException =
            listenToEventAction.Should().Throw<Exception>().Which;

        actualException.Should().BeSameAs(expected:innerException);
    }

    [Fact]
    public void ShouldRethrowOnListenToEventIfProcessorStartFails()
    {
        string inputName = "event-name";
        Mock<ServiceBusProcessor> serviceBusProcessorMock = new();
        Exception innerException = new("Processor start failure");

        serviceBusBrokerMock
            .Setup(broker => broker.CreateProcessor(inputName))
            .Returns(value:serviceBusProcessorMock.Object);

        serviceBusBrokerMock
            .Setup(broker => broker.StartProcessorAsync(inputName))
            .Returns(value:ValueTask.FromException(innerException));

        Action listenToEventAction = () =>
            serviceBusService.ListenToEvent<FakeObject>(
name:                inputName,
handler:                (_, _) => ValueTask.CompletedTask);

        Exception actualException =
            listenToEventAction.Should().Throw<Exception>().Which;

        actualException.Should().BeSameAs(expected:innerException);
    }
}