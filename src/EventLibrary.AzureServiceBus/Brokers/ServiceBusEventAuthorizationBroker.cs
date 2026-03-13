using EventLibrary.AzureServiceBus.Models;

namespace EventLibrary.AzureServiceBus.Brokers;

internal class ServiceBusEventAuthorizationBroker : IServiceBusEventAuthorizationBroker
{
    internal ServiceBusEventMessage Message { get; set; }

    public IServiceBusEventAuthInfo GetEventAuthInfo() =>
        Message?.AuthInfo;
}
