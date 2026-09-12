// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal interface IServiceBusEventAuthorizationBroker
{
    void SetEventMessage(ServiceBusEventMessage serviceBusEventMessage);

    IServiceBusEventAuthInfo GetEventAuthInfo();
}