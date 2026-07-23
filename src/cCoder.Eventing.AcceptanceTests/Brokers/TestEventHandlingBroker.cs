// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AcceptanceTests.Models;

namespace cCoder.Eventing.AcceptanceTests.Brokers;

internal sealed class TestEventHandlingBroker
{
    public IList<EventRecord> Records { get; } = [];
}