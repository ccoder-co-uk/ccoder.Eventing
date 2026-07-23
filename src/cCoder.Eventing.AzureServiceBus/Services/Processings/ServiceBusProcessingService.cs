// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Foundations;

namespace cCoder.Eventing.AzureServiceBus.Services.Processings;

internal sealed partial class ServiceBusProcessingService(
        Func<IServiceBusEventAuthInfo> getAuthInfo,
        IServiceBusService serviceBusService) 
            : IServiceBusProcessingService
{
    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);
            serviceBusService.ListenToEvent<T>(name:name, handler:handler);
        });

    public ValueTask RaiseEventAsync<T>(
        string name,
        T data) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, data]);

            IServiceBusEventAuthInfo authInfo = getAuthInfo();

            ServiceBusEventMessage<T> eventMessage = new()
            {
                AuthInfo = authInfo is null
                    ? null
                    : new ServiceBusEventAuthInfo { SSOUserId = authInfo.SSOUserId },
                Data = data
            };

            await RaiseEventInternalAsync(name:name, message:eventMessage);
        });

    public ValueTask RaiseEventAsync<T>(
        string name,
        ServiceBusEventMessage<T> message) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, message]);
            await RaiseEventInternalAsync(name:name, message:message);
        });

    public ValueTask RaiseEventsAsync<T>(
        string name,
        ServiceBusEventMessage<T>[] messages) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, messages]);

            foreach (ServiceBusEventMessage<T> message in messages)
            {
                await RaiseEventInternalAsync(name:name, message:message);
            }
        });

    private async ValueTask RaiseEventInternalAsync<T>(
        string name,
        ServiceBusEventMessage<T> message)
    {
        ValidateRequest(name:name, message:message);

        await serviceBusService.RaiseEventAsync(name:name, eventMessage:message);
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