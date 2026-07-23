// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Brokers;
using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Models;
using cCoder.Eventing.AzureServiceBus.AcceptanceTests.Services;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus.AcceptanceTests.Hubs;

public partial class AzureServiceBusEventHubTests
{
    [ConfigurationRequirement]
    public async Task ShouldRaiseEventsAsyncThroughHubAndPopulateScopedAuthInfo()
    {
        (ServiceProvider serviceProvider, string queueName) = CreateServiceProvider();
        await using (serviceProvider)
        {
            IAzureServiceBusEventHub eventHub =
                serviceProvider.GetRequiredService<IAzureServiceBusEventHub>();
            TestEventHandlingBroker broker =
                serviceProvider.GetRequiredService<TestEventHandlingBroker>();

            string payloadValueOne = $"payload-{Guid.NewGuid():N}";
            string payloadValueTwo = $"payload-{Guid.NewGuid():N}";
            string userIdOne = $"user-{Guid.NewGuid():N}";
            string userIdTwo = $"user-{Guid.NewGuid():N}";

            eventHub.ListenToEvent<TestPayload>(
                queueName,
                (scopedProvider, payload) =>
                {
                    TestEventHandlingService handlingService =
                        scopedProvider.GetRequiredService<TestEventHandlingService>();

                    return handlingService.HandleAsync(payload);
                });

            await eventHub.RaiseEventsAsync(
                queueName,
                [
                    CreateMessage(payloadValueOne, userIdOne),
                    CreateMessage(payloadValueTwo, userIdTwo)
                ]);

            IList<EventRecord> receivedRecords = await WaitForRecordsAsync(broker, 2);

            Assert.Contains(receivedRecords, record =>
                record.PayloadValue == payloadValueOne &&
                record.UserId == userIdOne);

            Assert.Contains(receivedRecords, record =>
                record.PayloadValue == payloadValueTwo &&
                record.UserId == userIdTwo);
        }
    }
}