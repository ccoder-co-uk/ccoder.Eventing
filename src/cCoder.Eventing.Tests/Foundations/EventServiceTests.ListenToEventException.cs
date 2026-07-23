// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceTests
{
    [Fact]
    public void ShouldRethrowOnListenToEventIfBrokerFails()
    {
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;
        Exception innerException = new("Broker failure");

        eventBrokerMock
            .Setup(broker => broker.ListenToEvent(inputName, inputHandler))
            .Throws(exception:innerException);

        Action listenToEventAction = () => eventService.ListenToEvent(name:inputName, handler:inputHandler);

        Exception actualException =
            listenToEventAction.Should().Throw<Exception>().Which;

        actualException.Should().BeSameAs(expected:innerException);
    }
}