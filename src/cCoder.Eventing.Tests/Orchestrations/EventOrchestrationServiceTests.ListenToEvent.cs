// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Services.Foundations;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
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
    public async Task ShouldListenToEventWithHandlingService()
    {
        // Given

        string inputName = "event-name";
        FakeObject inputMessage = new() { Name = "test" };
        Mock<IHandlingService> handlingServiceMock = new();

        IServiceProvider inputServiceProvider = new ServiceCollection()
            .AddSingleton(implementationInstance:handlingServiceMock.Object)
            .BuildServiceProvider();

        Func<IServiceProvider, FakeObject, ValueTask> internalHandler = null;
        IHandlingService actualHandlingService = null;
        FakeObject actualMessage = null;

        eventServiceProviderServiceMock
            .Setup(expression:service => service.ListenToEvent(
name: inputName,
handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
action: (_, handler) => internalHandler = handler);

        eventOrchestrationService.ListenToEvent<FakeObject, IHandlingService>(
name: inputName,
handler: (handlingService, message) =>
            {
                actualHandlingService = handlingService;
                actualMessage = message;
                return ValueTask.CompletedTask;
            });

        // When

        await internalHandler(arg1:inputServiceProvider, arg2:inputMessage);

        // Then

        actualHandlingService.Should()
            .BeSameAs(expected:handlingServiceMock.Object);

        actualMessage.Should()
            .BeSameAs(expected:inputMessage);
    }

    public interface IHandlingService;
}