using EventLibrary.Models;

namespace EventLibrary.AzureServiceBus.Services.Foundations;

public interface IServiceBusService
{
    ValueTask RaiseEventAsync<T>(string name, EventMessage<T> eventMessage);
}
