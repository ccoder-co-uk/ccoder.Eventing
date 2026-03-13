using EventLibrary.AzureServiceBus.Models;

namespace EventLibrary.AzureServiceBus;

public interface IAzureServiceBusEventHub
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, ServiceBusEventMessage<T> message);
    ValueTask RaiseEventsAsync<T>(string name, ServiceBusEventMessage<T>[] messages);
}
