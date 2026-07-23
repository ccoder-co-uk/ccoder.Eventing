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
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        eventOrchestrationService.ListenToEvent(name:inputName, handler:inputHandler);

        eventServiceProviderServiceMock.Verify(
expression:            service => service.ListenToEvent(inputName, inputHandler),
times:            Times.Once);
    }

    [Fact]
    public async Task ShouldListenToEventWithHandlingService()
    {
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
            .Setup(service => service.ListenToEvent(
                inputName,
                It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
action:                (_, handler) => internalHandler = handler);

        eventOrchestrationService.ListenToEvent<FakeObject, IHandlingService>(
name:            inputName,
handler:            (handlingService, message) =>
            {
                actualHandlingService = handlingService;
                actualMessage = message;
                return ValueTask.CompletedTask;
            });

        await internalHandler(arg1:inputServiceProvider, arg2:inputMessage);

        actualHandlingService.Should().BeSameAs(expected:handlingServiceMock.Object);
        actualMessage.Should().BeSameAs(expected:inputMessage);
    }

    public interface IHandlingService;
}