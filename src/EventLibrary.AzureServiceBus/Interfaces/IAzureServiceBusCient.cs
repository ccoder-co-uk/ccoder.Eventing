using EventLibrary.Objects;

namespace EventLibrary.AzureServiceBus.Interfaces
{
    public interface IAzureServiceBusCient
    {
        ValueTask RaiseEventAsync<T>(string name, EventMessage<T> eventMessage);
        ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] eventMessages);
    }
}
