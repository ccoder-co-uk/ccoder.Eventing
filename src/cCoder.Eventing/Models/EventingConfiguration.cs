// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public class EventingConfiguration
{
    public EventProvider[] EventProviders { get; set; } = [];
    public BulkEventProvider[] BulkEventProviders { get; set; } = [];
}