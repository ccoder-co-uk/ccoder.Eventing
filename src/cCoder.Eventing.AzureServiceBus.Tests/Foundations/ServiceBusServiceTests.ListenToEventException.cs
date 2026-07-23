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
            .Throws(innerException);

        Action listenToEventAction = () =>
            serviceBusService.ListenToEvent<FakeObject>(
                inputName,
                (_, _) => ValueTask.CompletedTask);

        Exception actualException =
            listenToEventAction.Should().Throw<Exception>().Which;

        actualException.Should().BeSameAs(innerException);
    }

    [Fact]
    public void ShouldRethrowOnListenToEventIfProcessorStartFails()
    {
        string inputName = "event-name";
        Mock<ServiceBusProcessor> serviceBusProcessorMock = new();
        Exception innerException = new("Processor start failure");

        serviceBusBrokerMock
            .Setup(broker => broker.CreateProcessor(inputName))
            .Returns(serviceBusProcessorMock.Object);

        serviceBusBrokerMock
            .Setup(broker => broker.StartProcessorAsync(inputName))
            .Returns(ValueTask.FromException(innerException));

        Action listenToEventAction = () =>
            serviceBusService.ListenToEvent<FakeObject>(
                inputName,
                (_, _) => ValueTask.CompletedTask);

        Exception actualException =
            listenToEventAction.Should().Throw<Exception>().Which;

        actualException.Should().BeSameAs(innerException);
    }
}