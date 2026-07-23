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
    public async Task ShouldRaiseEventAsyncAndReturnTrueWhenMatchingProviderExists()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        EventMessage<FakeObject> actualMessage = null;
        IServiceProvider actualServiceProvider = null;

        IEventProviderService eventProviderService = CreateEventProviderService(
eventProviders: new EventProvider<FakeObject>
            {
                Events = [inputName],
                SendHandler = (serviceProvider, _, message) =>
                {
                    actualServiceProvider = serviceProvider;
                    actualMessage = message;
                    return ValueTask.CompletedTask;
                }
            });

        // When

        bool handled = await eventProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        handled.Should()
            .BeTrue();

        actualMessage.Should()
            .BeSameAs(expected:inputMessage);

        actualServiceProvider.Should()
            .BeSameAs(expected:scopedServiceProviderMock.Object);

        serviceProviderBrokerMock.Verify(
expression: broker => broker.GetScopeForEvent(message:inputMessage),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncForEveryMatchingProvider()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        int callCount = 0;

        IEventProviderService eventProviderService = CreateEventProviderService(
            eventProviders:
            [
                new EventProvider<FakeObject>
            {
                Events = [inputName],
                SendHandler = (_, _, _) =>
                {
                    callCount++;
                    return ValueTask.CompletedTask;
                }
            },
                new EventProvider<FakeObject>
                {
                Events = [inputName],
                SendHandler = (_, _, _) =>
                {
                    callCount++;
                    return ValueTask.CompletedTask;
                }
                }
            ]);

        // When

        bool handled = await eventProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        handled.Should()
            .BeTrue();

        callCount.Should()
            .Be(expected:2);

        serviceProviderBrokerMock.Verify(
expression: broker => broker.GetScopeForEvent(message:inputMessage),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldReturnFalseWhenNoMatchingProviderExists()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        IEventProviderService eventProviderService = CreateEventProviderService(
eventProviders: new EventProvider<string>
            {
                Events = [inputName],
                SendHandler = (_, _, _) => ValueTask.CompletedTask
            });

        // When

        bool handled = await eventProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        handled.Should()
            .BeFalse();

        serviceProviderBrokerMock.Verify(
expression: broker => broker.GetScopeForEvent(message:It.IsAny<EventMessage>()),
times: Times.Never);
    }
}