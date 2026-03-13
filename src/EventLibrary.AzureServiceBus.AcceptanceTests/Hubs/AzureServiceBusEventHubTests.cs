using EventLibrary.AzureServiceBus.AcceptanceTests.Brokers;
using EventLibrary.AzureServiceBus.AcceptanceTests.Models;
using EventLibrary.AzureServiceBus.AcceptanceTests.Services;
using EventLibrary.AzureServiceBus.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.AzureServiceBus.AcceptanceTests.Hubs;

public partial class AzureServiceBusEventHubTests
{
    private static readonly TimeSpan RecordWaitTimeout = TimeSpan.FromSeconds(15);

    private const string ConnectionStringEnvironmentVariable =
        "EVENT_LIBRARY_AZURE_SERVICE_BUS_CONNECTION_STRING";

    private const string QueueNameEnvironmentVariable =
        "EVENT_LIBRARY_AZURE_SERVICE_BUS_QUEUE_NAME";

    private static async Task<EventRecord> WaitForSingleRecordAsync(
        TestEventHandlingBroker broker)
    {
        IList<EventRecord> records = await WaitForRecordsAsync(broker, 1);

        return records.Single();
    }

    private static async Task<IList<EventRecord>> WaitForRecordsAsync(
        TestEventHandlingBroker broker,
        int expectedCount)
    {
        DateTime timeoutAt = DateTime.UtcNow.Add(RecordWaitTimeout);

        while (DateTime.UtcNow < timeoutAt)
        {
            if (broker.Records.Count >= expectedCount)
            {
                return broker.Records.ToArray();
            }

            await Task.Delay(100);
        }

        return broker.Records.ToArray();
    }

    private static (ServiceProvider ServiceProvider, string QueueName) CreateServiceProvider()
    {
        string connectionString = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable)!;
        string queueName = Environment.GetEnvironmentVariable(QueueNameEnvironmentVariable)!;

        ServiceCollection services = new();

        services.AddLogging();
        services.AddAzureServiceBusEventing(connectionString);
        services.AddSingleton<TestEventHandlingBroker>();
        services.AddTransient<TestEventHandlingService>();

        return (services.BuildServiceProvider(), queueName);
    }

    private static ServiceBusEventMessage<TestPayload> CreateMessage(string payloadValue, string userId) =>
        new()
        {
            Data = new TestPayload { Value = payloadValue },
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = userId }
        };
}
