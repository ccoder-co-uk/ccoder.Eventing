using EventLibrary.Models;

namespace EventLibrary.AzureServiceBus.Services.Processings.Interfaces;

public interface IServiceBusProcessingService
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, T data);
    ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message);
    ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}
