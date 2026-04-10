namespace EventLibrary.Models;

public class EventingConfiguration
{
    public EventProvider[] EventProviders { get; set; } = [];
    public BulkEventProvider[] BulkEventProviders { get; set; } = [];
}
