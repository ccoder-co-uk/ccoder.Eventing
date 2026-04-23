using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Models;

namespace cCoder.Eventing.AzureServiceBus.AcceptanceTests.Brokers;

internal sealed class TestEventHandlingBroker
{
    private readonly List<EventRecord> records = [];

    public IReadOnlyList<EventRecord> Records
    {
        get
        {
            lock (records)
            {
                return records.ToArray();
            }
        }
    }

    public void AddRecord(EventRecord record)
    {
        lock (records)
        {
            records.Add(record);
        }
    }
}
