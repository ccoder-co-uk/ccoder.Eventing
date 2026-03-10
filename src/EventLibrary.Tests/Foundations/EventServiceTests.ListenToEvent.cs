using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Foundations;

public partial class EventServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        eventService.ListenToEvent(inputName, inputHandler);

        eventBrokerMock.Verify(
            broker => broker.ListenToEvent(inputName, inputHandler),
            Times.Once);
    }
}
