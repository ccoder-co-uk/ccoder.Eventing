// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public sealed class ServiceBusEventingConfiguration
{
    public ServiceBusEventingConfiguration()
    {
        ConnectionString = string.Empty;
        MaxConcurrency = 1;
    }

    public string ConnectionString { get; set; }
    public int MaxConcurrency { get; set; }
}