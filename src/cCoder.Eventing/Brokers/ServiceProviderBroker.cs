// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Brokers;

internal class ServiceProviderBroker(IServiceProvider serviceProvider) 
    : IServiceProviderBroker
{
    public IServiceScope GetScopeForEvent(EventMessage message)
    {
        IServiceScope scope = serviceProvider.CreateScope();

        IEventAuthorizationBroker authBroker =
            scope.ServiceProvider.GetService<IEventAuthorizationBroker>();

        authBroker.SetEventMessage(message: message);

        return scope;
    }

    public IServiceProvider GetServiceProvider() => 
        serviceProvider;

    public T GetService<T>() => 
        serviceProvider.GetService<T>();

    public T[] GetServices<T>() =>
        serviceProvider
            .GetServices<T>()
            .ToArray();

    public object GetService(Type type) => 
        serviceProvider.GetService(serviceType:type);
}