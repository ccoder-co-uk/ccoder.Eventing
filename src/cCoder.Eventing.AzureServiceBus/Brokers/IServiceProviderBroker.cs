using cCoder.Eventing.AzureServiceBus.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal interface IServiceProviderBroker
{
    IServiceScope GetScopeForEvent(ServiceBusEventMessage message);
    object GetService(Type type);
    T GetService<T>();
    IServiceProvider GetServiceProvider();
}
