// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.AcceptanceTests.Brokers;
using cCoder.Eventing.AcceptanceTests.Models;
using cCoder.Eventing.AcceptanceTests.Services;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AcceptanceTests.Hubs;

public partial class EventHubTests
{
    [Fact]
    public async Task ShouldRaiseEventsAsyncThroughHubAndPopulateScopedAuthInfo()
    {
        using ServiceProvider serviceProvider = CreateServiceProvider();
        IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

        eventHub.ListenToEvent<TestPayload, TestEventHandlingService>(
name:            EventName,
handler:            (handlingService, message) => handlingService.HandleAsync(message));

        await eventHub.RaiseEventsAsync(
name:            EventName,
messages:            [
                CreateMessage("payload-one", "event-user-one"),
                CreateMessage("payload-two", "event-user-two")
            ]);

        TestEventHandlingBroker state = serviceProvider.GetRequiredService<TestEventHandlingBroker>();

        Assert.Equal(expected:2, actual:state.Records.Count);
        Assert.Equal(expected:"payload-one", actual:state.Records[0].PayloadValue);
        Assert.Equal(expected:"event-user-one", actual:state.Records[0].UserId);
        Assert.Equal(expected:"payload-two", actual:state.Records[1].PayloadValue);
        Assert.Equal(expected:"event-user-two", actual:state.Records[1].UserId);
    }
}