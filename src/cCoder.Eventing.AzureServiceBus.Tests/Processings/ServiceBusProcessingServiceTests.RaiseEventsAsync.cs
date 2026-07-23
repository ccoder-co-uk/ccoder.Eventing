// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventsAsync()
    {
        // Given

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

        // When

        await serviceBusProcessingService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        serviceBusServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, eventMessage:It.IsAny<ServiceBusEventMessage<FakeObject>>()),
times: Times.Exactly(callCount:inputMessages.Length));
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncWhenMessageIsInvalid()
    {
        // Given

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

        // When

        Func<Task> raiseEventsAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        await raiseEventsAsyncTask.Should()
            .ThrowAsync<ServiceDependencyException>();

        serviceBusServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, eventMessage:It.IsAny<ServiceBusEventMessage<FakeObject>>()),
times: Times.Once);
    }
}