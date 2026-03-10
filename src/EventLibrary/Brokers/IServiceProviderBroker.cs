using EventLibrary.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.Brokers;

public interface IServiceProviderBroker
{
    IServiceScope GetScopeForEvent(EventMessage message);
    IServiceProvider GetServiceProvider();
    T GetService<T>();
    object GetService(Type type);
}
