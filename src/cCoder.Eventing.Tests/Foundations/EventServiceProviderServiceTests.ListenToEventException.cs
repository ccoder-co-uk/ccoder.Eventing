// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using cCoder.Eventing.Services.Processings;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    [Fact]
    public void ShouldRethrowOnListenToEventIfProcessingServiceFails()
    {
        // Given

        string inputName = "event-name";

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        Exception innerException = new("Processing failure");

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventProcessingServiceMock
            .Setup(expression:service => service.ListenToEvent(name:inputName, handler:inputHandler))
            .Throws(exception:innerException);

        // When

        Action listenToEventAction = () =>
            eventServiceProviderService.ListenToEvent(name:inputName, handler:inputHandler);

        // Then

        ServiceException actualException =
            listenToEventAction.Should()
                .Throw<ServiceException>()
                .Which;

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}