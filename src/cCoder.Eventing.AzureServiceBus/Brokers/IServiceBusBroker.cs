// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal interface IServiceBusBroker
{
    ValueTask SendAsync<T>(
        string name,
        ServiceBusEventMessage<T> eventMessage);

    void Listen<T>(
        string name,
        Func<ServiceBusEventMessage<T>, ValueTask> handler,
        Func<Exception, Task> errorHandler);
}