using EventLibrary.AzureServiceBus.Models;

namespace EventLibrary.AzureServiceBus.Services.Foundations;

internal interface IServiceBusService
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, ServiceBusEventMessage<T> eventMessage);
}
