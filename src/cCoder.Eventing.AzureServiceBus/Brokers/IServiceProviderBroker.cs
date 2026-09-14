// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.CodeAnalysis.Exposures;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal interface IServiceProviderBroker : IUtilityBroker
{
    IServiceScope GetScopeForEvent(ServiceBusEventMessage serviceBusEventMessage);
    object GetService(Type type);
    T GetService<T>();
    IServiceProvider GetServiceProvider();
}