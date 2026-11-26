// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Brokers;

internal sealed class LocalEventBroker(IServiceProvider services)
    : ILocalEventBroker
{
    public IEnumerable<ILocalEventHandler<T>> GetHandlers<T>(string name) =>
        services.GetKeyedServices<ILocalEventHandler<T>>(serviceKey: name);
}