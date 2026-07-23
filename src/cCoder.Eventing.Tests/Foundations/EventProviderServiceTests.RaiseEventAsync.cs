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
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };
        EventMessage<FakeObject> actualMessage = null;
        IServiceProvider actualServiceProvider = null;

        IEventProviderService eventProviderService = CreateEventProviderService(
eventProviders:            new EventProvider<FakeObject>
            {
                Events = [inputName],
                SendHandler = (serviceProvider, _, message) =>
                {
                    actualServiceProvider = serviceProvider;
                    actualMessage = message;
                    return ValueTask.CompletedTask;
                }
            });

        bool handled = await eventProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        handled.Should().BeTrue();
        actualMessage.Should().BeSameAs(expected:inputMessage);
        actualServiceProvider.Should().BeSameAs(expected:scopedServiceProviderMock.Object);

        serviceProviderBrokerMock.Verify(
expression:            broker => broker.GetScopeForEvent(inputMessage),
times:            Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncForEveryMatchingProvider()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };
        int callCount = 0;

        IEventProviderService eventProviderService = CreateEventProviderService(
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
            });

        bool handled = await eventProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        handled.Should().BeTrue();
        callCount.Should().Be(expected:2);

        serviceProviderBrokerMock.Verify(
expression:            broker => broker.GetScopeForEvent(inputMessage),
times:            Times.Once);
    }

    [Fact]
    public async Task ShouldReturnFalseWhenNoMatchingProviderExists()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        IEventProviderService eventProviderService = CreateEventProviderService(
eventProviders:            new EventProvider<string>
            {
                Events = [inputName],
                SendHandler = (_, _, _) => ValueTask.CompletedTask
            });

        bool handled = await eventProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        handled.Should().BeFalse();

        serviceProviderBrokerMock.Verify(
expression:            broker => broker.GetScopeForEvent(It.IsAny<EventMessage>()),
times:            Times.Never);
    }
}