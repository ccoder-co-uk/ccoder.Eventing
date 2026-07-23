// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Models;
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
        ServiceBusMessage actualMessage = null;

        ServiceBusEventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject { Name = "test" }
        };

        serviceBusBrokerMock
            .Setup(expression:broker => broker.SendMessageAsync(
name: inputName,
message: It.IsAny<ServiceBusMessage>()))
            .Callback<string, ServiceBusMessage>(action:(_, message) => actualMessage = message)
            .Returns(value:ValueTask.CompletedTask);

        // When

        await serviceBusService.RaiseEventAsync(name:inputName, eventMessage:inputEventMessage);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage.MessageId.Should()
            .Contain(expected:"user");

        actualMessage.MessageId.Should()
            .Contain(expected:nameof(FakeObject));

        serviceBusBrokerMock.Verify(
expression: broker => broker.SendMessageAsync(name:inputName, message:It.IsAny<ServiceBusMessage>()),
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

        Exception serviceBusException = new("Broker failure");

        serviceBusBrokerMock
            .Setup(expression:broker => broker.SendMessageAsync(name:inputName, message:It.IsAny<ServiceBusMessage>()))
            .Returns(value:new ValueTask(Task.FromException(exception:serviceBusException)));

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusService.RaiseEventAsync(name:inputName, eventMessage:inputEventMessage);

        // Then

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(testCode:raiseEventAsyncTask);

        actualException.Should()
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

        Exception innerException = new("Inner failure");
        Exception serviceBusException = new("Broker failure", innerException);

        serviceBusBrokerMock
            .Setup(expression:broker => broker.SendMessageAsync(name:inputName, message:It.IsAny<ServiceBusMessage>()))
            .Returns(value:new ValueTask(Task.FromException(exception:serviceBusException)));

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusService.RaiseEventAsync(name:inputName, eventMessage:inputEventMessage);

        // Then

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(testCode:raiseEventAsyncTask);

        actualException.Should()
            .BeSameAs(expected:serviceBusException);

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}