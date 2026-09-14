// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Orchestrations;

public partial class EventOrchestrationServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        // Given

        string inputName = "event-name";

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        // When

        eventOrchestrationService.ListenToEvent(name:inputName, handler:inputHandler);

        // Then

        eventProcessingServiceMock.Verify(
expression: service => service.ListenToEvent(name:inputName, handler:inputHandler),
times: Times.Once);
    }

    [Fact]
    public void ShouldResolveEventProcessingServiceOnceForMultipleListenersOfSameType()
    {
        // Given

        string inputName = "event-name";

        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        // When

        eventOrchestrationService.ListenToEvent(
            name: inputName,
            handler: inputHandler);

        eventOrchestrationService.ListenToEvent(
            name: inputName,
            handler: inputHandler);

        // Then

        serviceProviderBrokerMock.Verify(
            expression: broker =>
                broker.GetService<IEventProcessingService<FakeObject>>(),
            times: Times.Once);

        eventProcessingServiceMock.Verify(
            expression: service => service.ListenToEvent(
                name: inputName,
                handler: inputHandler),
            times: Times.Exactly(callCount: 2));
    }

    [Fact]
    public async Task ShouldListenToEventWithHandlingService()
    {
        // Given

        string inputName = "event-name";

        Mock<IHandlingService> handlingServiceMock = new();
        Mock<IServiceProvider> scopedServiceProviderMock = new();
        Func<IServiceProvider, FakeObject, ValueTask> internalHandler = null;

        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetRequiredService<IHandlingService>(
                serviceProvider: scopedServiceProviderMock.Object))
            .Returns(value: handlingServiceMock.Object);

        eventProcessingServiceMock
            .Setup(expression: service => service.ListenToEvent(
                name: inputName,
                handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
                action: (_, handler) => internalHandler = handler);

        IHandlingService actualHandlingService = null;

        Func<IHandlingService, FakeObject, ValueTask> inputHandler =
            (handlingService, _) =>
            {
                actualHandlingService = handlingService;
                return ValueTask.CompletedTask;
            };

        // When

        eventOrchestrationService.ListenToEvent<FakeObject, IHandlingService>(
            name: inputName,
            handler: inputHandler);

        await internalHandler(
            arg1: scopedServiceProviderMock.Object,
            arg2: new FakeObject());

        // Then

        actualHandlingService
            .Should()
            .BeSameAs(expected: handlingServiceMock.Object);
    }

    public interface IHandlingService;
}