// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Orchestrations;

public partial class EventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldThrowOnListenToEventWithHandlingServiceIfServiceIsMissing()
    {
        // Given

        string inputName = "event-name";
        FakeObject inputMessage = new() { Name = "test" };

        IServiceProvider inputServiceProvider = new ServiceCollection()
            .BuildServiceProvider();

        Func<IServiceProvider, FakeObject, ValueTask> internalHandler = null;

        eventServiceProviderServiceMock
            .Setup(expression:service => service.ListenToEvent(
name: inputName,
handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
action: (_, handler) => internalHandler = handler);

        eventOrchestrationService.ListenToEvent<FakeObject, IHandlingService>(
name: inputName,
handler: (_, _) => ValueTask.CompletedTask);

        // When

        Func<Task> invokeHandler = async () =>
            await internalHandler(arg1:inputServiceProvider, arg2:inputMessage);

        // Then

        await invokeHandler.Should()
            .ThrowAsync<InvalidOperationException>();
    }
}