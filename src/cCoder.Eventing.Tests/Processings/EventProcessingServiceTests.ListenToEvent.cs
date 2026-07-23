// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Processings;

public partial class EventProcessingServiceTests
{
    [Fact]
    public async Task ShouldListenToEvent()
    {
        // Given

        string inputName = "event-name";
        IServiceProvider inputServiceProvider = Mock.Of<IServiceProvider>();
        FakeObject inputData = new() { Name = "test" };
        Func<IServiceProvider, FakeObject, ValueTask> forwardedHandler = null;
        FakeObject actualData = null;

        eventServiceMock
            .Setup(expression:service => service.ListenToEvent(
name: inputName,
handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
action: (_, handler) => forwardedHandler = handler);

        eventProcessingService.ListenToEvent(
name: inputName,
handler: (_, data) =>
            {
                actualData = data;
                return ValueTask.CompletedTask;
            });

        // When

        await forwardedHandler(arg1:inputServiceProvider, arg2:inputData);

        // Then

        actualData.Should()
            .BeSameAs(expected:inputData);
    }

    [Fact]
    public async Task ShouldListenToEventAndSkipNullMessages()
    {
        // Given

        string inputName = "event-name";
        IServiceProvider inputServiceProvider = Mock.Of<IServiceProvider>();
        Func<IServiceProvider, FakeObject, ValueTask> forwardedHandler = null;
        bool handlerWasCalled = false;

        eventServiceMock
            .Setup(expression:service => service.ListenToEvent(
name: inputName,
handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
action: (_, handler) => forwardedHandler = handler);

        eventProcessingService.ListenToEvent(
name: inputName,
handler: (_, _) =>
            {
                handlerWasCalled = true;
                return ValueTask.CompletedTask;
            });

        // When

        await forwardedHandler(arg1:inputServiceProvider, arg2:null);

        // Then

        handlerWasCalled.Should()
            .BeFalse();
    }
}