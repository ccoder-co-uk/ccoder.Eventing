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
            [],
            [
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

        bool handled = await eventProviderService.RaiseEventsAsync(inputName, inputMessages);

        handled.Should().BeTrue();
        actualMessages.Should().BeSameAs(inputMessages);
        actualServiceProvider.Should().BeSameAs(scopedServiceProviderMock.Object);

        serviceProviderBrokerMock.Verify(
            broker => broker.GetScopeForEvent(inputMessages[0]),
            Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventsAsyncForEveryMatchingBulkProvider()
    {
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
            [],
            [
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

        bool handled = await eventProviderService.RaiseEventsAsync(inputName, inputMessages);

        handled.Should().BeTrue();
        callCount.Should().Be(2);
    }

    [Fact]
    public async Task ShouldReturnFalseWhenNoMatchingBulkProviderExists()
    {
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
            [],
            [
                new BulkEventProvider<string>
                {
                    Events = [inputName],
                    Handler = (_, _) => ValueTask.CompletedTask
                }
            ]);

        bool handled = await eventProviderService.RaiseEventsAsync(inputName, inputMessages);

        handled.Should().BeFalse();

        serviceProviderBrokerMock.Verify(
            broker => broker.GetScopeForEvent(It.IsAny<EventMessage>()),
            Times.Never);
    }
}
