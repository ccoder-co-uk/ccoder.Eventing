// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.Models;

public interface IServiceBusEventAuthInfo
{
    string SSOUserId { get; set; }
}