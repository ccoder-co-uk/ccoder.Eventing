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
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        eventService.ListenToEvent(name:inputName, handler:inputHandler);

        serviceProviderBrokerMock.Verify(
expression:            broker => broker.GetService<IEventBroker<FakeObject>>(),
times:            Times.Once);

        eventBrokerMock.Verify(
expression:            broker => broker.ListenToEvent(inputName, inputHandler),
times:            Times.Once);
    }
}