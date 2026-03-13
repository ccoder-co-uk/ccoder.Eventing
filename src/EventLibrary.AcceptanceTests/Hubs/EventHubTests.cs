using EventLibrary;
using EventLibrary.AcceptanceTests.Brokers;
using EventLibrary.Models;
using EventLibrary.AcceptanceTests.Models;
using EventLibrary.AcceptanceTests.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.AcceptanceTests.Hubs;

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