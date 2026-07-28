// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public class EventingConfiguration
{
    public EventingConfiguration()
    {
        ProviderType = "Http";
        Http = new HttpEventingConfiguration();
        ServiceBus = new ServiceBusEventingConfiguration();
        EventProviders = [];
        BulkEventProviders = [];
    }

    public string ProviderType { get; set; }
    public HttpEventingConfiguration Http { get; set; }
    public ServiceBusEventingConfiguration ServiceBus { get; set; }
    public EventProvider[] EventProviders { get; set; }
    public BulkEventProvider[] BulkEventProviders { get; set; }
}

public sealed class HttpEventingConfiguration
{
    public HttpEventingConfiguration()
    {
        HubUrl = string.Empty;
        MaxConcurrency = 1;
    }

    public string HubUrl { get; set; }
    public int MaxConcurrency { get; set; }
}

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