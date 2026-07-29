// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Moq;
using cCoder.Eventing.AzureServiceBus.Models;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        // Given

        string inputName = "event-name";

        serviceBusBrokerMock
            .Setup(expression: broker => broker.Listen<FakeObject>(
                name: inputName,
                handler: It.IsAny<Func<ServiceBusEventMessage<FakeObject>, ValueTask>>(),
                errorHandler: It.IsAny<Func<Exception, Task>>()));

        // When

        ServiceBusService.ListenToEvent<FakeObject>(
name: inputName,
handler: (_, _) => ValueTask.CompletedTask);

        // Then

        serviceBusBrokerMock.Verify(
            expression: broker => broker.Listen<FakeObject>(
                name: inputName,
                handler: It.IsAny<Func<ServiceBusEventMessage<FakeObject>, ValueTask>>(),
                errorHandler: It.IsAny<Func<Exception, Task>>()),
            times: Times.Once);
    }
}