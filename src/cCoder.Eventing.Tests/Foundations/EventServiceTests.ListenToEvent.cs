// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        // Given

        string inputName = "event-name";

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        // When

        eventService.ListenToEvent(name:inputName, handler:inputHandler);

        // Then

        serviceProviderBrokerMock.Verify(
expression: broker => broker.GetService<IEventBroker<FakeObject>>(),
times: Times.Once);

        eventBrokerMock.Verify(
expression: broker => broker.ListenToEvent(name:inputName, handler:inputHandler),
times: Times.Once);
    }
}