// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AzureServiceBus.Models;

public class AzureServiceBusEventingConfiguration
{
    public string ConnectionString { get; set; }
    public int MaxConcurrency { get; set; }
}