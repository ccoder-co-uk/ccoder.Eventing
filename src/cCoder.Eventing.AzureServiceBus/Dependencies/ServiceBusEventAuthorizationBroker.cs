// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Dependencies;

internal class ServiceBusEventAuthorizationBroker : IServiceBusEventAuthorizationBroker
{
    internal ServiceBusEventMessage Message { get; set; }

    public IServiceBusEventAuthInfo GetEventAuthInfo() =>
        Message?.AuthInfo;
}