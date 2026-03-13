using EventLibrary.AzureServiceBus.Models;

namespace EventLibrary.AzureServiceBus.Brokers;

internal interface IServiceBusEventAuthorizationBroker
{
    IServiceBusEventAuthInfo GetEventAuthInfo();
}
