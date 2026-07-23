// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    [Fact]
    public void ShouldRethrowOnListenToEventIfBrokerFails()
    {
        // Given

        string inputName = "event-name";
        Exception innerException = new("Broker failure");

        serviceBusBrokerMock
            .Setup(expression:broker => broker.CreateProcessor(name:inputName))
            .Throws(exception:innerException);

        // When

        Action listenToEventAction = () =>
            serviceBusService.ListenToEvent<FakeObject>(
name: inputName,
handler: (_, _) => ValueTask.CompletedTask);

        // Then

        ServiceException actualException =
            listenToEventAction.Should()
                .Throw<ServiceException>()
                .Which;

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }

    [Fact]
    public void ShouldRethrowOnListenToEventIfProcessorStartFails()
    {
        // Given

        string inputName = "event-name";
        Mock<ServiceBusProcessor> serviceBusProcessorMock = new();
        Exception innerException = new("Processor start failure");

        serviceBusBrokerMock
            .Setup(expression:broker => broker.CreateProcessor(name:inputName))
            .Returns(value:serviceBusProcessorMock.Object);

        serviceBusBrokerMock
            .Setup(expression:broker => broker.StartProcessorAsync(name:inputName))
            .Returns(value:ValueTask.FromException(exception:innerException));

        // When

        Action listenToEventAction = () =>
            serviceBusService.ListenToEvent<FakeObject>(
name: inputName,
handler: (_, _) => ValueTask.CompletedTask);

        // Then

        ServiceException actualException =
            listenToEventAction.Should()
                .Throw<ServiceException>()
                .Which;

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}