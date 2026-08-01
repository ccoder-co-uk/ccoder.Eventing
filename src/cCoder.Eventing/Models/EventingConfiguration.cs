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