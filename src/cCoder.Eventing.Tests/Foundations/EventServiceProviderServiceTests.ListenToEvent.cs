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
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent(name:inputName, handler:inputHandler);

        eventProcessingServiceMock.Verify(
expression:            service => service.ListenToEvent(inputName, inputHandler),
times:            Times.Once);
    }

    [Fact]
    public void ShouldOnlyResolveProcessingServiceOnceForType()
    {
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent(name:inputName, handler:inputHandler);
        eventServiceProviderService.ListenToEvent(name:inputName, handler:inputHandler);

        serviceProviderBrokerMock.Verify(
expression:            broker => broker.GetService<IEventProcessingService<FakeObject>>(),
times:            Times.Once);
    }
}