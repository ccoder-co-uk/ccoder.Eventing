using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Brokers;

internal class ServiceProviderBroker(IServiceProvider serviceProvider) 
    : IServiceProviderBroker
{
    public IServiceScope GetScopeForEvent(EventMessage message)
    {
        IServiceScope scope = serviceProvider.CreateScope();

        EventAuthorizationBroker authBroker =
            scope.ServiceProvider.GetService<IEventAuthorizationBroker>() 
                as EventAuthorizationBroker;

        authBroker.Message = message;

        return scope;
    }

    public IServiceProvider GetServiceProvider() => 
        serviceProvider;

    public T GetService<T>() => 
        serviceProvider.GetService<T>();

    public object GetService(Type type) => 
        serviceProvider.GetService(type);
}
