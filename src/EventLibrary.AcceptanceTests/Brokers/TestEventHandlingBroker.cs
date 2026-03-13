using EventLibrary.AcceptanceTests.Models;

namespace EventLibrary.AcceptanceTests.Brokers;

internal sealed class TestEventHandlingBroker
{
    public IList<EventRecord> Records { get; } = [];
}
