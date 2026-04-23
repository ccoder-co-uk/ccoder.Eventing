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
        string inputName = "event-name";
        ServiceBusMessage actualMessage = null;
        ServiceBusEventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject { Name = "test" }
        };

        serviceBusBrokerMock
            .Setup(broker => broker.SendMessageAsync(
                inputName,
                It.IsAny<ServiceBusMessage>()))
            .Callback<string, ServiceBusMessage>((_, message) => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        await serviceBusService.RaiseEventAsync(inputName, inputEventMessage);

        actualMessage.Should().NotBeNull();
        actualMessage.MessageId.Should().Contain("user");
        actualMessage.MessageId.Should().Contain(nameof(FakeObject));

        serviceBusBrokerMock.Verify(
            broker => broker.SendMessageAsync(inputName, It.IsAny<ServiceBusMessage>()),
            Times.Once);
    }

    [Fact]
    public async Task ShouldRethrowOnRaiseEventAsyncWhenBrokerFails()
    {
        string inputName = "event-name";
        ServiceBusEventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject()
        };

        Exception serviceBusException = new("Broker failure");

        serviceBusBrokerMock
            .Setup(broker => broker.SendMessageAsync(inputName, It.IsAny<ServiceBusMessage>()))
            .Returns(new ValueTask(Task.FromException(serviceBusException)));

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusService.RaiseEventAsync(inputName, inputEventMessage);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(raiseEventAsyncTask);

        actualException.Should().BeSameAs(serviceBusException);
    }

    [Fact]
    public async Task ShouldRethrowOnRaiseEventAsyncWhenBrokerFailsWithInnerException()
    {
        string inputName = "event-name";
        ServiceBusEventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject()
        };

        Exception innerException = new("Inner failure");
        Exception serviceBusException = new("Broker failure", innerException);

        serviceBusBrokerMock
            .Setup(broker => broker.SendMessageAsync(inputName, It.IsAny<ServiceBusMessage>()))
            .Returns(new ValueTask(Task.FromException(serviceBusException)));

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusService.RaiseEventAsync(inputName, inputEventMessage);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(raiseEventAsyncTask);

        actualException.Should().BeSameAs(serviceBusException);
        actualException.InnerException.Should().BeSameAs(innerException);
    }
}
