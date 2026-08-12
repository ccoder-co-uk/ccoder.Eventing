// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    [Fact]
    public void ShouldWrapArgumentExceptionOnListenToEvent()
    {
        // Given

        ArgumentException dependencyException = new(message: "dependency failure");

        serviceBusBrokerMock
            .Setup(expression: broker => broker.Listen<FakeObject>(
                name: It.IsAny<string>(),
                handler: It.IsAny<Func<ServiceBusEventMessage<FakeObject>, ValueTask>>(),
                errorHandler: It.IsAny<Func<Exception, Task>>()))
            .Throws(exception: dependencyException);

        // When

        Action listenAction = () => ServiceBusService.ListenToEvent<FakeObject>(
            name: "event-name",
            handler: (_, _) => ValueTask.CompletedTask);

        // Then

        ServiceValidationException actualException =
            Assert.Throws<ServiceValidationException>(testCode: listenAction);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public void ShouldWrapInvalidOperationExceptionOnListenToEvent()
    {
        // Given

        InvalidOperationException dependencyException = new(message: "dependency failure");

        serviceBusBrokerMock
            .Setup(expression: broker => broker.Listen<FakeObject>(
                name: It.IsAny<string>(),
                handler: It.IsAny<Func<ServiceBusEventMessage<FakeObject>, ValueTask>>(),
                errorHandler: It.IsAny<Func<Exception, Task>>()))
            .Throws(exception: dependencyException);

        // When

        Action listenAction = () => ServiceBusService.ListenToEvent<FakeObject>(
            name: "event-name",
            handler: (_, _) => ValueTask.CompletedTask);

        // Then

        ServiceDependencyException actualException =
            Assert.Throws<ServiceDependencyException>(testCode: listenAction);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public void ShouldRethrowOnListenToEventIfBrokerFails()
    {
        // Given

        string inputName = "event-name";
        Exception innerException = new(message: "Broker failure");

        serviceBusBrokerMock
            .Setup(expression: broker => broker.Listen<FakeObject>(
                name: inputName,
                handler: It.IsAny<Func<ServiceBusEventMessage<FakeObject>, ValueTask>>(),
                errorHandler: It.IsAny<Func<Exception, Task>>()))
            .Throws(exception:innerException);

        // When

        Action listenToEventAction = () =>
            ServiceBusService.ListenToEvent<FakeObject>(
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