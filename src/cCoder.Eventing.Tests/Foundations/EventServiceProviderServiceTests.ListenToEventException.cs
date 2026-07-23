// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;
        Exception innerException = new("Processing failure");

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(eventProcessingServiceMock.Object);

        eventProcessingServiceMock
            .Setup(service => service.ListenToEvent(inputName, inputHandler))
            .Throws(innerException);

        Action listenToEventAction = () =>
            eventServiceProviderService.ListenToEvent(inputName, inputHandler);

        Exception actualException =
            listenToEventAction.Should().Throw<Exception>().Which;

        actualException.Should().BeSameAs(innerException);
    }
}