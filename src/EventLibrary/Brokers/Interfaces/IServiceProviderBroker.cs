using EventLibrary.Objects;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.Brokers.Interfaces
{
    public interface IServiceProviderBroker
    {
        IServiceScope GetScopeForEvent(EventMessage message);
        IServiceProvider GetServiceProvider();
        T GetService<T>();
        object GetService(Type type);
    }
}