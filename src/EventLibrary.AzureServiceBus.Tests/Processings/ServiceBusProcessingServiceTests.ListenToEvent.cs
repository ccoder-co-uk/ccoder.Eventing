using FluentAssertions;
using Xunit;

namespace EventLibrary.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    [Fact]
    public void ShouldThrowOnListenToEvent()
    {
        Action listenToEventAction = () =>
            serviceBusProcessingService.ListenToEvent<FakeObject>(
                "event-name",
                (_, _) => ValueTask.CompletedTask);

        listenToEventAction.Should().Throw<NotSupportedException>();
    }
}
