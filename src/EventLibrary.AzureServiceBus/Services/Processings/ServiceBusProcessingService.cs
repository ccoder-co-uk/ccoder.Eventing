using EventLibrary.AzureServiceBus.Services.Foundations.Interfaces;
using EventLibrary.AzureServiceBus.Services.Processings.Interfaces;
using EventLibrary.Models;
using EventLibrary.Models.Interfaces;

namespace EventLibrary.AzureServiceBus.Services.Processings;

public class ServiceBusProcessingService : IServiceBusProcessingService
{
    private readonly Func<IEventAuthInfo> getAuthInfo;
    private readonly IServiceBusService serviceBusService;

    public ServiceBusProcessingService(
        Func<IEventAuthInfo> getAuthInfo,
        IServiceBusService serviceBusService)
    {
        this.getAuthInfo = getAuthInfo;
        this.serviceBusService = serviceBusService;
    }

    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
        throw new NotSupportedException("Azure Service Bus event hubs do not support in-process listeners.");

    public async ValueTask RaiseEventAsync<T>(string name, T data)
    {
        EventMessage<T> eventMessage = new()
        {
            AuthInfo = getAuthInfo(),
            Data = data
        };

        ValidateRequest(name, eventMessage);
        await serviceBusService.RaiseEventAsync(name, eventMessage);
    }

    public async ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message)
    {
        ValidateRequest(name, message);
        await serviceBusService.RaiseEventAsync(name, message);
    }

    public async ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages)
    {
        foreach (EventMessage<T> message in messages)
        {
            await RaiseEventAsync(name, message);
        }
    }

    private static void ValidateRequest<T>(string name, EventMessage<T> message)
    {
        if (name is null)
        {
            throw new InvalidOperationException("You must provide an event name when raising events.");
        }

        if (message is null)
        {
            throw new InvalidOperationException("You must provide a message when raising events.");
        }

        if (message.Data is null)
        {
            throw new InvalidOperationException("You must provide some message data when raising events.");
        }

        if (message.AuthInfo is null)
        {
            throw new InvalidOperationException("You must provide some message auth information when raising events.");
        }
    }
}
