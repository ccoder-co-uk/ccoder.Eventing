// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventProviderServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventsAsyncAndReturnTrueWhenMatchingBulkProviderExists()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        EventMessage<FakeObject>[] actualMessages = null;
        IServiceProvider actualServiceProvider = null;

        IEventProviderService eventProviderService = CreateEventProviderService(
eventProviders: [],
bulkEventProviders: [
                new BulkEventProvider<FakeObject>
                {
                    Events = [inputName],
                    Handler = (serviceProvider, messages) =>
                    {
                        actualServiceProvider = serviceProvider;
                        actualMessages = messages;
                        return ValueTask.CompletedTask;
                    }
                }
            ]);

        // When

        bool handled = await eventProviderService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        handled.Should()
            .BeTrue();

        actualMessages.Should()
            .BeSameAs(expected:inputMessages);

        actualServiceProvider.Should()
            .BeSameAs(expected:scopedServiceProviderMock.Object);

        serviceProviderBrokerMock.Verify(
expression: broker => broker.GetScopeForEvent(message:inputMessages[0]),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventsAsyncForEveryMatchingBulkProvider()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        int callCount = 0;

        IEventProviderService eventProviderService = CreateEventProviderService(
eventProviders: [],
bulkEventProviders: [
                new BulkEventProvider<FakeObject>
                {
                    Events = [inputName],
                    Handler = (_, _) =>
                    {
                        callCount++;
                        return ValueTask.CompletedTask;
                    }
                },
                new BulkEventProvider<FakeObject>
                {
                    Events = [inputName],
                    Handler = (_, _) =>
                    {
                        callCount++;
                        return ValueTask.CompletedTask;
                    }
                }
            ]);

        // When

        bool handled = await eventProviderService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        handled.Should()
            .BeTrue();

        callCount.Should()
            .Be(expected:2);
    }

    [Fact]
    public async Task ShouldReturnFalseWhenNoMatchingBulkProviderExists()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        IEventProviderService eventProviderService = CreateEventProviderService(
eventProviders: [],
bulkEventProviders: [
                new BulkEventProvider<string>
                {
                    Events = [inputName],
                    Handler = (_, _) => ValueTask.CompletedTask
                }
            ]);

        // When

        bool handled = await eventProviderService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        handled.Should()
            .BeFalse();

        serviceProviderBrokerMock.Verify(
expression: broker => broker.GetScopeForEvent(message:It.IsAny<EventMessage>()),
times: Times.Never);
    }
}