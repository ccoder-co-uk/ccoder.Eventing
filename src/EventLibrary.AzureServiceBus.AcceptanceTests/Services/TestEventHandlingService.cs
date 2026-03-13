using EventLibrary.AzureServiceBus.AcceptanceTests.Brokers;
using EventLibrary.AzureServiceBus.AcceptanceTests.Models;
using EventLibrary.AzureServiceBus.Models;

namespace EventLibrary.AzureServiceBus.AcceptanceTests.Services;

internal sealed class TestEventHandlingService(
    TestEventHandlingBroker broker,
    IServiceBusEventAuthInfo authInfo)
{
    public ValueTask HandleAsync(TestPayload payload)
    {
        broker.AddRecord(new EventRecord
        {
            PayloadValue = payload.Value,
            UserId = authInfo.SSOUserId
        });

        return ValueTask.CompletedTask;
    }
}
