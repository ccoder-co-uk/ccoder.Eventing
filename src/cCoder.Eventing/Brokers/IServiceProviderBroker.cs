// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.CodeAnalysis.Exposures;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Brokers;

public interface IServiceProviderBroker : IUtilityBroker
{
    IServiceScope GetScopeForEvent(EventMessage eventMessage);
    IServiceProvider GetServiceProvider();
    T GetService<T>();
    T GetRequiredService<T>(IServiceProvider serviceProvider);
    T[] GetServices<T>();
    object GetService(Type type);
}