// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventsAsync()
    {
        string inputName = "event-name";
        ServiceBusEventMessage<FakeObject>[] inputMessages =
        [
            new ServiceBusEventMessage<FakeObject>
            {
                AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user-one" },
                Data = new FakeObject()
            },
            new ServiceBusEventMessage<FakeObject>
            {
                AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user-two" },
                Data = new FakeObject()
            }
        ];

        await serviceBusProcessingService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        serviceBusServiceMock.Verify(
expression:            service => service.RaiseEventAsync(inputName, It.IsAny<ServiceBusEventMessage<FakeObject>>()),
times:            Times.Exactly(inputMessages.Length));
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncWhenMessageIsInvalid()
    {
        string inputName = "event-name";
        ServiceBusEventMessage<FakeObject>[] inputMessages =
        [
            new ServiceBusEventMessage<FakeObject>
            {
                AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user-one" },
                Data = new FakeObject()
            },
            new ServiceBusEventMessage<FakeObject>
            {
                AuthInfo = null,
                Data = new FakeObject()
            }
        ];

        Func<Task> raiseEventsAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        await raiseEventsAsyncTask.Should().ThrowAsync<InvalidOperationException>();

        serviceBusServiceMock.Verify(
expression:            service => service.RaiseEventAsync(inputName, It.IsAny<ServiceBusEventMessage<FakeObject>>()),
times:            Times.Once);
    }
}