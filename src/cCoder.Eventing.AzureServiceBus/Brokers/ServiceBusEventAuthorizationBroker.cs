// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal class ServiceBusEventAuthorizationBroker : IServiceBusEventAuthorizationBroker
{
    private ServiceBusEventMessage message;

    public void SetEventMessage(ServiceBusEventMessage message) =>
        this.message = message;

    public IServiceBusEventAuthInfo GetEventAuthInfo() =>
        message?.AuthInfo;
}