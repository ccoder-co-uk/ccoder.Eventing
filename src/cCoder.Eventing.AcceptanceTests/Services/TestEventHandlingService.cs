// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AcceptanceTests.Brokers;
using cCoder.Eventing.AcceptanceTests.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.AcceptanceTests.Services;

internal sealed class TestEventHandlingService(
    TestEventHandlingBroker state,
    IEventAuthInfo authInfo)
{
    public ValueTask HandleAsync(TestPayload payload)
    {
        state.Records.Add(item:new EventRecord
        {
            PayloadValue = payload.Value,
            UserId = authInfo.SSOUserId
        });

        return ValueTask.CompletedTask;
    }
}