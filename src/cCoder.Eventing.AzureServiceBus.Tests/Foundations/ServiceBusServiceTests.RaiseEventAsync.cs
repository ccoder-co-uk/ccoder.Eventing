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
    public async Task ShouldRaiseEventAsync()
    {
        // Given

        string inputName = "event-name";

        ServiceBusEventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject { Name = "test" }
        };

        serviceBusBrokerMock
            .Setup(expression: broker => broker.SendAsync(
                name: inputName,
                eventMessage: inputEventMessage))
            .Returns(value: ValueTask.CompletedTask);

        // When

        await ServiceBusService.RaiseEventAsync(name:inputName, eventMessage:inputEventMessage);

        // Then

        serviceBusBrokerMock.Verify(
            expression: broker => broker.SendAsync(
                name: inputName,
                eventMessage: inputEventMessage),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldRethrowOnRaiseEventAsyncWhenBrokerFails()
    {
        // Given

        string inputName = "event-name";

        ServiceBusEventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject()
        };

        Exception serviceBusException = new(message: "Broker failure");

        serviceBusBrokerMock
            .Setup(expression: broker => broker.SendAsync(
                name: inputName,
                eventMessage: inputEventMessage))
            .Returns(value: ValueTask.FromException(exception: serviceBusException));

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await ServiceBusService.RaiseEventAsync(name:inputName, eventMessage:inputEventMessage);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode:raiseEventAsyncTask);

        actualException.InnerException.Should()
            .BeSameAs(expected:serviceBusException);
    }

    [Fact]
    public async Task ShouldRethrowOnRaiseEventAsyncWhenBrokerFailsWithInnerException()
    {
        // Given

        string inputName = "event-name";

        ServiceBusEventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject()
        };

        Exception innerException = new(message: "Inner failure");

        Exception serviceBusException = new(
            message: "Broker failure",
            innerException: innerException);

        serviceBusBrokerMock
            .Setup(expression: broker => broker.SendAsync(
                name: inputName,
                eventMessage: inputEventMessage))
            .Returns(value: ValueTask.FromException(exception: serviceBusException));

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await ServiceBusService.RaiseEventAsync(name:inputName, eventMessage:inputEventMessage);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode:raiseEventAsyncTask);

        actualException.InnerException.Should()
            .BeSameAs(expected:serviceBusException);

        actualException.InnerException!.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}