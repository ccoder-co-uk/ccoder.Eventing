using EventLibrary;
using EventLibrary.AcceptanceTests.Brokers;
using EventLibrary.AcceptanceTests.Models;
using EventLibrary.AcceptanceTests.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.AcceptanceTests.Hubs;

public partial class EventHubTests
{
    [Fact]
    public async Task ShouldRaiseEventsAsyncThroughHubAndPopulateScopedAuthInfo()
    {
        using ServiceProvider serviceProvider = CreateServiceProvider();
        IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

        eventHub.ListenToEvent<TestPayload, TestEventHandlingService>(
            EventName,
            (handlingService, message) => handlingService.HandleAsync(message));

        await eventHub.RaiseEventsAsync(
            EventName,
            [
                CreateMessage("payload-one", "event-user-one"),
                CreateMessage("payload-two", "event-user-two")
            ]);

        TestEventHandlingBroker state = serviceProvider.GetRequiredService<TestEventHandlingBroker>();

        Assert.Equal(2, state.Records.Count);
        Assert.Equal("payload-one", state.Records[0].PayloadValue);
        Assert.Equal("event-user-one", state.Records[0].UserId);
        Assert.Equal("payload-two", state.Records[1].PayloadValue);
        Assert.Equal("event-user-two", state.Records[1].UserId);
    }
}
