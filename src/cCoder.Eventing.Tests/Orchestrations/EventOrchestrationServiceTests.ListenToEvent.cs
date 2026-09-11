// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Services.Foundations;
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

        eventServiceProviderServiceMock.Verify(
expression: service => service.ListenToEvent(name:inputName, handler:inputHandler),
times: Times.Once);
    }

    [Fact]
    public void ShouldListenToEventWithHandlingService()
    {
        // Given

        string inputName = "event-name";

        Func<IHandlingService, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        // When

        eventOrchestrationService.ListenToEvent<FakeObject, IHandlingService>(
            name: inputName,
            handler: inputHandler);

        // Then

        eventServiceProviderServiceMock.Verify(
            expression: service =>
                service.ListenToEvent<FakeObject, IHandlingService>(
                    name: inputName,
                    handler: inputHandler),
            times: Times.Once);
    }

    public interface IHandlingService;
}