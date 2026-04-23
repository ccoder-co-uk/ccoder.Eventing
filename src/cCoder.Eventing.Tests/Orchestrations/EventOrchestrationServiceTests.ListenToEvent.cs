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

        eventOrchestrationService.ListenToEvent(inputName, inputHandler);

        eventServiceProviderServiceMock.Verify(
            service => service.ListenToEvent(inputName, inputHandler),
            Times.Once);
    }

    [Fact]
    public async Task ShouldListenToEventWithHandlingService()
    {
        string inputName = "event-name";
        FakeObject inputMessage = new() { Name = "test" };
        Mock<IHandlingService> handlingServiceMock = new();
        IServiceProvider inputServiceProvider = new ServiceCollection()
            .AddSingleton(handlingServiceMock.Object)
            .BuildServiceProvider();

        Func<IServiceProvider, FakeObject, ValueTask> internalHandler = null;
        IHandlingService actualHandlingService = null;
        FakeObject actualMessage = null;

        eventServiceProviderServiceMock
            .Setup(service => service.ListenToEvent(
                inputName,
                It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
                (_, handler) => internalHandler = handler);

        eventOrchestrationService.ListenToEvent<FakeObject, IHandlingService>(
            inputName,
            (handlingService, message) =>
            {
                actualHandlingService = handlingService;
                actualMessage = message;
                return ValueTask.CompletedTask;
            });

        await internalHandler(inputServiceProvider, inputMessage);

        actualHandlingService.Should().BeSameAs(handlingServiceMock.Object);
        actualMessage.Should().BeSameAs(inputMessage);
    }

    public interface IHandlingService;
}
