// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.Services.Processings;

internal sealed partial class ServiceBusProcessingService
{
    private static void Validate(params object[] inputs)
    {
        if (inputs.Any(predicate: input => input is null))
        {
            throw new ArgumentNullException(nameof(inputs));
        }
    }
}