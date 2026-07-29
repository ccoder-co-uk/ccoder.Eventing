// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.AcceptanceTests.Hubs;

internal sealed class ConfigurationRequirementAttribute : FactAttribute
{
    public ConfigurationRequirementAttribute()
    {
        AcceptanceTestConfiguration configuration =
            AcceptanceTestConfiguration.Load();

        if (!configuration.IsComplete)
        {
            Skip =
                $"Set {AcceptanceTestConfiguration.ConnectionStringVariableName} and {AcceptanceTestConfiguration.QueueNameVariableName} to run Azure Service Bus acceptance tests.";
        }
    }
}