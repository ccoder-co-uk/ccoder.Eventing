using cCoder.Eventing.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent(inputName, inputHandler);

        eventProcessingServiceMock.Verify(
            service => service.ListenToEvent(inputName, inputHandler),
            Times.Once);
    }

    [Fact]
    public void ShouldOnlyResolveProcessingServiceOnceForType()
    {
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent(inputName, inputHandler);
        eventServiceProviderService.ListenToEvent(inputName, inputHandler);

        serviceProviderBrokerMock.Verify(
            broker => broker.GetService<IEventProcessingService<FakeObject>>(),
            Times.Once);
    }
}
