// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus;

public interface IAzureServiceBusEventHub
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, ServiceBusEventMessage<T> message);
    ValueTask RaiseEventsAsync<T>(string name, ServiceBusEventMessage<T>[] messages);
}
