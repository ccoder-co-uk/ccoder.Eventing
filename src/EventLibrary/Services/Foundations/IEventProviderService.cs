using EventLibrary.Models;

namespace EventLibrary.Services.Foundations;

internal interface IEventProviderService
{
    ValueTask<bool> RaiseEventAsync<T>(string name, EventMessage<T> message);
    ValueTask<bool> RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}
