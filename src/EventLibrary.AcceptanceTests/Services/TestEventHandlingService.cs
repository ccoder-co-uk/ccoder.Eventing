using EventLibrary.AcceptanceTests.Brokers;
using EventLibrary.AcceptanceTests.Models;
using EventLibrary.Models;

namespace EventLibrary.AcceptanceTests.Services;

internal sealed class TestEventHandlingService(
    TestEventHandlingBroker state,
    IEventAuthInfo authInfo)
{
    public ValueTask HandleAsync(TestPayload payload)
    {
        state.Records.Add(new EventRecord
        {
            PayloadValue = payload.Value,
            UserId = authInfo.SSOUserId
        });

        return ValueTask.CompletedTask;
    }
}
