// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal class ServiceProviderBroker(IServiceProvider serviceProvider) : IServiceProviderBroker
{
    public IServiceScope GetScopeForEvent(ServiceBusEventMessage message)
    {
        IServiceScope scope = serviceProvider.CreateScope();

        IServiceBusEventAuthorizationBroker authBroker =
            scope.ServiceProvider.GetService<IServiceBusEventAuthorizationBroker>();

        authBroker.SetEventMessage(message: message);

        return scope;
    }

    public IServiceProvider GetServiceProvider() =>
        serviceProvider;

    public T GetService<T>() =>
        serviceProvider.GetService<T>();

    public object GetService(Type type) =>
        serviceProvider.GetService(serviceType:type);
}