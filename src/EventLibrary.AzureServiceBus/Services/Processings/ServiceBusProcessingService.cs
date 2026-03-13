using EventLibrary.AzureServiceBus.Models;
using EventLibrary.AzureServiceBus.Services.Foundations;

namespace EventLibrary.AzureServiceBus.Services.Processings;

internal class ServiceBusProcessingService(
        Func<IServiceBusEventAuthInfo> getAuthInfo,
        IServiceBusService serviceBusService) 
            : IServiceBusProcessingService
{
    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler) =>
        serviceBusService.ListenToEvent<T>(name, handler);

    public async ValueTask RaiseEventAsync<T>(string name, T data)
    {
        IServiceBusEventAuthInfo authInfo = getAuthInfo();
        ServiceBusEventMessage<T> eventMessage = new()
        {
            AuthInfo = authInfo is null
                ? null
                : new ServiceBusEventAuthInfo { SSOUserId = authInfo.SSOUserId },
            Data = data
        };

        ValidateRequest(name, eventMessage);
        await serviceBusService.RaiseEventAsync(name, eventMessage);
    }

    public async ValueTask RaiseEventAsync<T>(string name, ServiceBusEventMessage<T> message)
    {
        ValidateRequest(name, message);
        await serviceBusService.RaiseEventAsync(name, message);
    }

    public async ValueTask RaiseEventsAsync<T>(string name, ServiceBusEventMessage<T>[] messages)
    {
        foreach (ServiceBusEventMessage<T> message in messages)
        {
            await RaiseEventAsync(name, message);
        }
    }

    private static void ValidateRequest<T>(string name, ServiceBusEventMessage<T> message)
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
