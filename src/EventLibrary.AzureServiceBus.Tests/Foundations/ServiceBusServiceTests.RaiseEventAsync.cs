using Azure.Messaging.ServiceBus;
using EventLibrary.Models;
using EventLibrary.Models.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsync()
    {
        string inputName = "event-name";
        ServiceBusMessage actualMessage = null;
        EventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(auth => auth.SSOUserId == "user"),
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
    public async Task ShouldThrowWrappedExceptionOnRaiseEventAsyncWhenBrokerFails()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(auth => auth.SSOUserId == "user"),
            Data = new FakeObject()
        };

        Exception serviceBusException = new("Broker failure");

        serviceBusBrokerMock
            .Setup(broker => broker.SendMessageAsync(inputName, It.IsAny<ServiceBusMessage>()))
            .Returns(new ValueTask(Task.FromException(serviceBusException)));

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusService.RaiseEventAsync(inputName, inputEventMessage);

        InvalidOperationException actualException =
            await Assert.ThrowsAsync<InvalidOperationException>(raiseEventAsyncTask);

        actualException.InnerException.Should().BeSameAs(serviceBusException);
    }

    [Fact]
    public async Task ShouldThrowWrappedExceptionOnRaiseEventAsyncWhenBrokerFailsWithInnerException()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputEventMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(auth => auth.SSOUserId == "user"),
            Data = new FakeObject()
        };

        Exception innerException = new("Inner failure");
        Exception serviceBusException = new("Broker failure", innerException);

        serviceBusBrokerMock
            .Setup(broker => broker.SendMessageAsync(inputName, It.IsAny<ServiceBusMessage>()))
            .Returns(new ValueTask(Task.FromException(serviceBusException)));

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusService.RaiseEventAsync(inputName, inputEventMessage);

        InvalidOperationException actualException =
            await Assert.ThrowsAsync<InvalidOperationException>(raiseEventAsyncTask);

        actualException.InnerException.Should().BeSameAs(serviceBusException);
        actualException.InnerException.InnerException.Should().BeSameAs(innerException);
    }
}
