// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using cCoder.Eventing.Brokers;

namespace cCoder.Eventing;

public static class LocalEventBrokerExtensions
{
    /// <summary>Awaits matching handlers in registration order. A handler exception stops dispatch.</summary>
    public static async ValueTask DispatchAsync<T>(this ILocalEventBroker broker, string name, T data)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(argument: name);
        ArgumentNullException.ThrowIfNull(argument: data);

        foreach (ILocalEventHandler<T> handler in broker.GetHandlers<T>(name: name))
        {
            await handler.HandleAsync(data: data);
        }
    }
}