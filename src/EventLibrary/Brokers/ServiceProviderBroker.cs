using EventLibrary.Brokers.Interfaces;
using EventLibrary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventLibrary.Brokers;

public class ServiceProviderBroker : IServiceProviderBroker
{
    private readonly IServiceProvider serviceProvider;
    private readonly ILogger<ServiceProviderBroker> log;

    public ServiceProviderBroker(
        IServiceProvider serviceProvider,
        ILogger<ServiceProviderBroker> log)
    {
        this.serviceProvider = serviceProvider;
        this.log = log;
    }

    public IServiceScope GetScopeForEvent(EventMessage message)
    {
        log.LogDebug("Creating event scope for user {UserId}", message?.AuthInfo?.SSOUserId ?? "Guest");

        IServiceScope scope = serviceProvider.CreateScope();

        EventAuthorizationBroker authBroker =
            scope.ServiceProvider.GetService<IEventAuthorizationBroker>() as EventAuthorizationBroker;

        authBroker.Message = message;

        return scope;
    }

    public IServiceProvider GetServiceProvider() => serviceProvider;

    public T GetService<T>() => serviceProvider.GetService<T>();

    public object GetService(Type type) => serviceProvider.GetService(type);
}
