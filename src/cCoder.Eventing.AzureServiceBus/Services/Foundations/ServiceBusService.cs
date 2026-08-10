// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Models;
using Microsoft.Extensions.DependencyInjection;
using cCoder.Eventing.AzureServiceBus.Brokers.Loggings;

namespace cCoder.Eventing.AzureServiceBus.Services.Foundations;

internal sealed partial class ServiceBusService(
        IServiceBusBroker serviceBusBroker,
        IServiceProviderBroker serviceProviderBroker,
        ILoggingBroker log) : IServiceBusService
{
    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            try
            {
                serviceBusBroker.Listen<T>(
                    name: name,
                    handler: message => HandleServiceBusMessage(
                        serviceProviderBroker: serviceProviderBroker,
                        handler: handler,
                        message: message),
                    errorHandler: exception =>
                        HandleServiceBusError(
                            name: name,
                            exception: exception));
            }
            catch (Exception ex)
            {
                log.LogError(
                    exception: ex,
                    message: "Exception thrown whilst listening to {Name} event:\n{Message}\n{StackTrace}",
                    args: [name, ex.Message, ex.StackTrace]);

                throw;
            }
        });

    public ValueTask RaiseEventAsync<T>(
        string name,
        ServiceBusEventMessage<T> eventMessage) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, eventMessage]);

            try
            {
                await serviceBusBroker.SendAsync(
                    name: name,
                    eventMessage: eventMessage);
            }
            catch (Exception ex)
            {
                log.LogError(
                    exception: ex,
                    message: "Exception thrown whilst raising {Name} event:\n{Message}\n{StackTrace}",
                    args: [name, ex.Message, ex.StackTrace]);

                if (ex.InnerException is not null)
                {
                    log.LogError(
                        exception: ex.InnerException,
                        message: "Inner Exception:\n{Message}\n{StackTrace}",
                        args: [ex.InnerException.Message, ex.InnerException.StackTrace]);
                }

                throw;
            }
        });

    private async ValueTask HandleServiceBusMessage<T>(
        IServiceProviderBroker serviceProviderBroker,
        Func<IServiceProvider, T, ValueTask> handler,
        ServiceBusEventMessage<T> message)
    {
        try
        {
            using IServiceScope scope = serviceProviderBroker
                .GetScopeForEvent(message:message);

            await handler(arg1:scope.ServiceProvider, arg2:message.Data);
        }
        catch (Exception ex)
        {
            log.LogError(
                exception: ex,
                message: "Exception thrown whilst handling service bus message\n{Message}\n{StackTrace}",
                args: [ex.Message, ex.StackTrace]);

            throw;
        }
    }

    private Task HandleServiceBusError(string name, Exception exception)
    {
        log.LogError(
            exception: exception,
            message: "Exception thrown whilst listening to {Name} event:\n{Message}\n{StackTrace}",
            args: [name, exception.Message, exception.StackTrace]);

        return Task.CompletedTask;
    }
}