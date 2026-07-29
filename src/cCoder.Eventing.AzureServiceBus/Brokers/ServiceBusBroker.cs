// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Dependencies;
using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal sealed class ServiceBusBroker(
    ServiceBusDependency serviceBusDependency) : IServiceBusBroker
{
    public ValueTask SendAsync<T>(
        string name,
        ServiceBusEventMessage<T> eventMessage) =>
        serviceBusDependency.SendAsync(
            name: name,
            eventMessage: eventMessage);

    public void Listen<T>(
        string name,
        Func<ServiceBusEventMessage<T>, ValueTask> handler,
        Func<Exception, Task> errorHandler) =>
        serviceBusDependency.Listen(
            name: name,
            handler: handler,
            errorHandler: errorHandler);
}