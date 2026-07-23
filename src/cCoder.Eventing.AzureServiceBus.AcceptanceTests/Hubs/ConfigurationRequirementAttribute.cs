// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.AcceptanceTests.Hubs;

internal sealed class ConfigurationRequirementAttribute : FactAttribute
{
    private const string ConnectionStringEnvironmentVariable =
        "EVENT_LIBRARY_AZURE_SERVICE_BUS_CONNECTION_STRING";

    private const string QueueNameEnvironmentVariable =
        "EVENT_LIBRARY_AZURE_SERVICE_BUS_QUEUE_NAME";

    public ConfigurationRequirementAttribute()
    {
        if (string.IsNullOrWhiteSpace(value:Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable)) ||
            string.IsNullOrWhiteSpace(value:Environment.GetEnvironmentVariable(QueueNameEnvironmentVariable)))
        {
            Skip =
                $"Set {ConnectionStringEnvironmentVariable} and {QueueNameEnvironmentVariable} to run Azure Service Bus acceptance tests.";
        }
    }
}