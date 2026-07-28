// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Brokers;

public interface IServiceProviderBroker
{
    IServiceScope GetScopeForEvent(EventMessage message);
    IServiceProvider GetServiceProvider();
    T GetService<T>();
    T[] GetServices<T>();
    object GetService(Type type);
}