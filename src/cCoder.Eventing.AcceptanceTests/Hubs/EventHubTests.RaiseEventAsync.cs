using cCoder.Eventing;
using cCoder.Eventing.AcceptanceTests.Brokers;
using cCoder.Eventing.AcceptanceTests.Models;
using cCoder.Eventing.AcceptanceTests.Services;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AcceptanceTests.Hubs;

public partial class EventHubTests
{
    [Fact]
    public async Task ShouldRaiseEventAsyncThroughHubAndPopulateScopedAuthInfo()
    {
        using ServiceProvider serviceProvider = CreateServiceProvider();
        IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

        eventHub.ListenToEvent<TestPayload, TestEventHandlingService>(
            EventName,
            (handlingService, message) => handlingService.HandleAsync(message));

        await eventHub.RaiseEventAsync(
            EventName,
            CreateMessage("payload-value", "event-user"));

        TestEventHandlingBroker state = serviceProvider.GetRequiredService<TestEventHandlingBroker>();

        Assert.Single(state.Records);
        Assert.Equal("payload-value", state.Records[0].PayloadValue);
        Assert.Equal("event-user", state.Records[0].UserId);
    }
}
