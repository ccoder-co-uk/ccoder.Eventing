using EventLibrary.AzureServiceBus.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.AzureServiceBus.Brokers;

internal interface IServiceProviderBroker
{
    IServiceScope GetScopeForEvent(ServiceBusEventMessage message);
    object GetService(Type type);
    T GetService<T>();
    IServiceProvider GetServiceProvider();
}
