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
        string inputName = "event-name";
        FakeObject inputMessage = new() { Name = "test" };
        IServiceProvider inputServiceProvider = new ServiceCollection()
            .BuildServiceProvider();

        Func<IServiceProvider, FakeObject, ValueTask> internalHandler = null;

        eventServiceProviderServiceMock
            .Setup(service => service.ListenToEvent(
                inputName,
                It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
                (_, handler) => internalHandler = handler);

        eventOrchestrationService.ListenToEvent<FakeObject, IHandlingService>(
            inputName,
            (_, _) => ValueTask.CompletedTask);

        Func<Task> invokeHandler = async () =>
            await internalHandler(inputServiceProvider, inputMessage);

        await invokeHandler.Should().ThrowAsync<InvalidOperationException>();
    }
}
