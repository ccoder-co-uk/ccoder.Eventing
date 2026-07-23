// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.AzureServiceBus.Services.Foundations;

internal class ServiceBusService(
        IServiceBusBroker serviceBusBroker,
        IServiceProviderBroker serviceProviderBroker,
        ILogger<ServiceBusService> log) : IServiceBusService
{
    public void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler)
    {
        try
        {
            ServiceBusProcessor processor = serviceBusBroker.CreateProcessor(name);

            processor.ProcessMessageAsync += (ProcessMessageEventArgs messageDetails) =>
                HandleServiceBusMessage(serviceProviderBroker, handler, messageDetails);

            processor.ProcessErrorAsync += (ProcessErrorEventArgs problemDetails) =>
                HandleServiceBusError(name, problemDetails);

            serviceBusBroker
                .StartProcessorAsync(name)
                .AsTask()
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst listening to {Name} event:\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }

    public async ValueTask RaiseEventAsync<T>(string name, ServiceBusEventMessage<T> eventMessage)
    {
        try
        {
            ServiceBusMessage message = new()
            {
                Body = new BinaryData(eventMessage),
                MessageId = $"{eventMessage.AuthInfo.SSOUserId}_{typeof(T).Name}_{Guid.NewGuid()}"
            };

            await serviceBusBroker.SendMessageAsync(name, message);
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst raising {Name} event:\n{Message}\n{StackTrace}",
                name,
                ex.Message,
                ex.StackTrace);

            if (ex.InnerException is not null)
            {
                log.LogError(
                    ex.InnerException,
                    "Inner Exception:\n{Message}\n{StackTrace}",
                    ex.InnerException.Message,
                    ex.InnerException.StackTrace);
            }

            throw;
        }
    }

    async Task HandleServiceBusMessage<T>(
        IServiceProviderBroker serviceProviderBroker,
        Func<IServiceProvider, T, ValueTask> handler,
        ProcessMessageEventArgs messageDetails)
    {
        try
        {
            ServiceBusEventMessage<T> message = messageDetails
                .Message
                .Body
                .ToObjectFromJson<ServiceBusEventMessage<T>>();

            using IServiceScope scope = serviceProviderBroker
                .GetScopeForEvent(message);

            await handler(scope.ServiceProvider, message.Data);
        }
        catch (Exception ex)
        {
            log.LogError(
                ex,
                "Exception thrown whilst handling service bus message\n{Message}\n{StackTrace}",
                ex.Message,
                ex.StackTrace);

            throw;
        }
    }

    Task HandleServiceBusError(string name, ProcessErrorEventArgs problemDetails)
    {
        log.LogError(
            problemDetails.Exception,
            "Exception thrown whilst listening to {Name} event:\n{Message}\n{StackTrace}",
            name,
            problemDetails.Exception.Message,
            problemDetails.Exception.StackTrace);

        return Task.CompletedTask;
    }
}