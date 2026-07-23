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
    public async Task ShouldRaiseEventAsyncThroughHubAndPopulateScopedAuthInfo()
    {
        using ServiceProvider serviceProvider = CreateServiceProvider();
        IEventHub eventHub = serviceProvider.GetRequiredService<IEventHub>();

        eventHub.ListenToEvent<TestPayload, TestEventHandlingService>(
name:            EventName,
handler:            (handlingService, message) => handlingService.HandleAsync(message));

        await eventHub.RaiseEventAsync(
name:            EventName,
message:            CreateMessage("payload-value", "event-user"));

        TestEventHandlingBroker state = serviceProvider.GetRequiredService<TestEventHandlingBroker>();

        Assert.Single(collection:state.Records);
        Assert.Equal(expected:"payload-value", actual:state.Records[0].PayloadValue);
        Assert.Equal(expected:"event-user", actual:state.Records[0].UserId);
    }
}