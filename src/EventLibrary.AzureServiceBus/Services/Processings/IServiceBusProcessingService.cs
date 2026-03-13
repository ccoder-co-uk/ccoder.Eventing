using EventLibrary.AzureServiceBus.Models;

namespace EventLibrary.AzureServiceBus.Services.Processings;

internal interface IServiceBusProcessingService
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, T data);
    ValueTask RaiseEventAsync<T>(string name, ServiceBusEventMessage<T> message);
    ValueTask RaiseEventsAsync<T>(string name, ServiceBusEventMessage<T>[] messages);
}
