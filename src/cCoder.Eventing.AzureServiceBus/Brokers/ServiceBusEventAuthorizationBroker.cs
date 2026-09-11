// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal class ServiceBusEventAuthorizationBroker : IServiceBusEventAuthorizationBroker
{
    private ServiceBusEventMessage serviceBusEventMessage;

    public void SetEventMessage(ServiceBusEventMessage serviceBusEventMessage) =>
        this.serviceBusEventMessage = serviceBusEventMessage;

    public IServiceBusEventAuthInfo GetEventAuthInfo() =>
        serviceBusEventMessage?.AuthInfo;
}