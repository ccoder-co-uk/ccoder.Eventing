// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        // Given

        string inputName = "event-name";

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        // When

        eventServiceProviderService.ListenToEvent(name:inputName, handler:inputHandler);

        // Then

        eventProcessingServiceMock.Verify(
expression: service => service.ListenToEvent(name:inputName, handler:inputHandler),
times: Times.Once);
    }

    [Fact]
    public void ShouldOnlyResolveProcessingServiceOnceForType()
    {
        // Given

        string inputName = "event-name";

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent(name:inputName, handler:inputHandler);
        // When

        eventServiceProviderService.ListenToEvent(name:inputName, handler:inputHandler);

        // Then

        serviceProviderBrokerMock.Verify(
expression: broker => broker.GetService<IEventProcessingService<FakeObject>>(),
times: Times.Once);
    }
}