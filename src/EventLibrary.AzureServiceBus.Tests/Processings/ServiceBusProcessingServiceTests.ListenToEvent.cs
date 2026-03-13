using EventLibrary.AzureServiceBus.Models;
using Moq;
using Xunit;

namespace EventLibrary.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    [Fact]
    public void ShouldListenToEvent()
    {
        string inputName = "event-name";
        Func<IServiceProvider, FakeObject, ValueTask> inputHandler =
            (_, _) => ValueTask.CompletedTask;

        serviceBusServiceMock
            .Setup(service => service.ListenToEvent(
                inputName,
                inputHandler));

        serviceBusProcessingService.ListenToEvent(inputName, inputHandler);

        serviceBusServiceMock.Verify(
            service => service.ListenToEvent(inputName, inputHandler),
            Times.Once);
    }
}
