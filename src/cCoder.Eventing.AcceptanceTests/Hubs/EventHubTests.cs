// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing;
using cCoder.Eventing.AcceptanceTests.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.AcceptanceTests.Models;
using cCoder.Eventing.AcceptanceTests.Services;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AcceptanceTests.Hubs;

public partial class EventHubTests
{
    private const string EventName = "test-event";

    private static ServiceProvider CreateServiceProvider()
    {
        ServiceCollection services = new();

        services.AddLogging();
        services.AddSingleton<TestEventHandlingBroker>();
        services.AddTransient<TestEventHandlingService>();
        services.AddEventing();
        services.AddEventingForType<TestPayload>();

        return services.BuildServiceProvider();
    }

    private static EventMessage<TestPayload> CreateMessage(
        string payloadValue, 
        string userId) => new()
    {
        Data = new TestPayload { Value = payloadValue },
        AuthInfo = new EventAuthInfo { SSOUserId = userId }
    };
}