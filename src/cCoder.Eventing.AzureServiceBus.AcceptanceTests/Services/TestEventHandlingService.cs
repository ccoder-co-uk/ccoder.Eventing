using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Brokers;
using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Models;
using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.AcceptanceTests.Services;

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
