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
name: queueName,
handler: (scopedProvider, payload) =>
                {
                    TestEventHandlingService handlingService =
                        scopedProvider.GetRequiredService<TestEventHandlingService>();

                    return handlingService.HandleAsync(payload:payload);
                });

            await eventHub.RaiseEventAsync(name:queueName, message:CreateMessage(payloadValue:payloadValue, userId:userId));

            EventRecord record = await WaitForSingleRecordAsync(broker:broker);

            Assert.Equal(expected:payloadValue, actual:record.PayloadValue);
            Assert.Equal(expected:userId, actual:record.UserId);
        }
    }
}