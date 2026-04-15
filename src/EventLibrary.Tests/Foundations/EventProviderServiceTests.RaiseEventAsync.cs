using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Foundations;

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
            new EventProvider<FakeObject>
            {
                Events = [inputName],
                SendHandler = (serviceProvider, _, message) =>
                {
                    actualServiceProvider = serviceProvider;
                    actualMessage = message;
                    return ValueTask.CompletedTask;
                }
            });

        bool handled = await eventProviderService.RaiseEventAsync(inputName, inputMessage);

        handled.Should().BeTrue();
        actualMessage.Should().BeSameAs(inputMessage);
        actualServiceProvider.Should().BeSameAs(scopedServiceProviderMock.Object);

        serviceProviderBrokerMock.Verify(
            broker => broker.GetScopeForEvent(inputMessage),
            Times.Once);
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

        bool handled = await eventProviderService.RaiseEventAsync(inputName, inputMessage);

        handled.Should().BeTrue();
        callCount.Should().Be(2);

        serviceProviderBrokerMock.Verify(
            broker => broker.GetScopeForEvent(inputMessage),
            Times.Once);
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
            new EventProvider<string>
            {
                Events = [inputName],
                SendHandler = (_, _, _) => ValueTask.CompletedTask
            });

        bool handled = await eventProviderService.RaiseEventAsync(inputName, inputMessage);

        handled.Should().BeFalse();

        serviceProviderBrokerMock.Verify(
            broker => broker.GetScopeForEvent(It.IsAny<EventMessage>()),
            Times.Never);
    }
}
