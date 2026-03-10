using EventLibrary.Models;
using EventLibrary.Models.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Processings;

public partial class EventProcessingServiceTests
{
    [Fact]
    public async Task ShouldListenToEvent()
    {
        string inputName = "event-name";
        IServiceProvider inputServiceProvider = Mock.Of<IServiceProvider>();
        FakeObject inputData = new() { Name = "test" };
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = inputData
        };

        Func<IServiceProvider, EventMessage<FakeObject>, ValueTask> forwardedHandler = null;
        FakeObject actualData = null;

        eventServiceMock
            .Setup(service => service.ListenToEvent(
                inputName,
                It.IsAny<Func<IServiceProvider, EventMessage<FakeObject>, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, EventMessage<FakeObject>, ValueTask>>(
                (_, handler) => forwardedHandler = handler);

        eventProcessingService.ListenToEvent(
            inputName,
            (_, data) =>
            {
                actualData = data;
                return ValueTask.CompletedTask;
            });

        await forwardedHandler(inputServiceProvider, inputMessage);

        actualData.Should().BeSameAs(inputData);
    }

    [Fact]
    public async Task ShouldListenToEventAndSkipNullMessages()
    {
        string inputName = "event-name";
        IServiceProvider inputServiceProvider = Mock.Of<IServiceProvider>();
        Func<IServiceProvider, EventMessage<FakeObject>, ValueTask> forwardedHandler = null;
        bool handlerWasCalled = false;

        eventServiceMock
            .Setup(service => service.ListenToEvent(
                inputName,
                It.IsAny<Func<IServiceProvider, EventMessage<FakeObject>, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, EventMessage<FakeObject>, ValueTask>>(
                (_, handler) => forwardedHandler = handler);

        eventProcessingService.ListenToEvent(
            inputName,
            (_, _) =>
            {
                handlerWasCalled = true;
                return ValueTask.CompletedTask;
            });

        await forwardedHandler(inputServiceProvider, null);

        handlerWasCalled.Should().BeFalse();
    }
}
