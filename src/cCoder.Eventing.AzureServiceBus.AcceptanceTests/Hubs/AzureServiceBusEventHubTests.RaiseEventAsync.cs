using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Brokers;
using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Models;
using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Services;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus.AcceptanceTests.Hubs;

public partial class AzureServiceBusEventHubTests
{
    [ConfigurationRequirement]
    public async Task ShouldRaiseEventAsyncThroughHubAndPopulateScopedAuthInfo()
    {
        (ServiceProvider serviceProvider, string queueName) = CreateServiceProvider();
        await using (serviceProvider)
        {
            IAzureServiceBusEventHub eventHub =
                serviceProvider.GetRequiredService<IAzureServiceBusEventHub>();
            TestEventHandlingBroker broker =
                serviceProvider.GetRequiredService<TestEventHandlingBroker>();

            string payloadValue = $"payload-{Guid.NewGuid():N}";
            string userId = $"user-{Guid.NewGuid():N}";

            eventHub.ListenToEvent<TestPayload>(
                queueName,
                (scopedProvider, payload) =>
                {
                    TestEventHandlingService handlingService =
                        scopedProvider.GetRequiredService<TestEventHandlingService>();

                    return handlingService.HandleAsync(payload);
                });

            await eventHub.RaiseEventAsync(queueName, CreateMessage(payloadValue, userId));

            EventRecord record = await WaitForSingleRecordAsync(broker);

            Assert.Equal(payloadValue, record.PayloadValue);
            Assert.Equal(userId, record.UserId);
        }
    }
}
