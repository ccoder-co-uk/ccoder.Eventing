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
            body: new BinaryData(eventMessage),
            messageId: $"{eventMessage.AuthInfo.SSOUserId}_{typeof(T).Name}_{Guid.NewGuid()}");

    public void Listen<T>(
        string name,
        Func<ServiceBusEventMessage<T>, ValueTask> handler,
        Func<Exception, Task> errorHandler) =>
        serviceBusDependency.Listen(
            name: name,
            handler: body => handler(
                arg: body.ToObjectFromJson<ServiceBusEventMessage<T>>()),
            errorHandler: errorHandler);
}