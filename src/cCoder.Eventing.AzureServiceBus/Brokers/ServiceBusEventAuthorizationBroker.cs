// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal class ServiceBusEventAuthorizationBroker : IServiceBusEventAuthorizationBroker
{
    internal ServiceBusEventMessage Message { get; set; }

    public IServiceBusEventAuthInfo GetEventAuthInfo() =>
        Message?.AuthInfo;
}