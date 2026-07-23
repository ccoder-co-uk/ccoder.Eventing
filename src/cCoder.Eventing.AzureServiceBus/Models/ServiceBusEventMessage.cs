// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.Models;

public class ServiceBusEventMessage<T> : ServiceBusEventMessage
{
    public T Data { get; set; }
}

public abstract class ServiceBusEventMessage
{
    public ServiceBusEventAuthInfo AuthInfo { get; set; }
}