using cCoder.Eventing.Brokers;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        eventService.ListenToEvent(inputName, inputHandler);

        serviceProviderBrokerMock.Verify(
            broker => broker.GetService<IEventBroker<FakeObject>>(),
            Times.Once);

        eventBrokerMock.Verify(
            broker => broker.ListenToEvent(inputName, inputHandler),
            Times.Once);
    }
}
