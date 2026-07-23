// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceTests
{
    [Fact]
    public void ShouldRethrowOnListenToEventIfBrokerFails()
    {
        // Given

        string inputName = "event-name";

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        Exception innerException = new("Broker failure");

        eventBrokerMock
            .Setup(expression:broker => broker.ListenToEvent(name:inputName, handler:inputHandler))
            .Throws(exception:innerException);

        // When

        Action listenToEventAction = () => eventService.ListenToEvent(name:inputName, handler:inputHandler);

        // Then

        ServiceException actualException =
            listenToEventAction.Should()
                .Throw<ServiceException>()
                .Which;

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}