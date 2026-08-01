// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.Models;

public abstract class ServiceBusEventMessage
{
    public ServiceBusEventAuthInfo AuthInfo { get; set; }
}