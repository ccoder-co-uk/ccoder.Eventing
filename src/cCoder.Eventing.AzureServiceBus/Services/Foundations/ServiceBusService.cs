// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.AzureServiceBus.Services.Foundations;

internal sealed partial class ServiceBusService(
        IServiceBusBroker serviceBusBroker,
        IServiceProviderBroker serviceProviderBroker,
        ILogger<ServiceBusService> log) : IServiceBusService
{
    private readonly IDictionary<string, ServiceBusSender> senders =
        new Dictionary<string, ServiceBusSender>();
    private readonly IDictionary<string, ServiceBusProcessor> processors =
        new Dictionary<string, ServiceBusProcessor>();
    private readonly ISet<string> startedProcessors = new HashSet<string>();

    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            try
            {
                ServiceBusProcessor processor = GetOrCreateProcessor(name: name);

                processor.ProcessMessageAsync += (ProcessMessageEventArgs messageDetails) =>
                    HandleServiceBusMessage(serviceProviderBroker:serviceProviderBroker, handler:handler, messageDetails:messageDetails);

                processor.ProcessErrorAsync += (ProcessErrorEventArgs problemDetails) =>
                    HandleServiceBusError(name:name, problemDetails:problemDetails);

                StartProcessor(name: name, processor: processor);
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
                ServiceBusMessage message = new()
                {
                    Body = new BinaryData(eventMessage),
                    MessageId = $"{eventMessage.AuthInfo.SSOUserId}_{typeof(T)
                        .Name}_{Guid.NewGuid()}"
                };

                ServiceBusSender sender = GetOrCreateSender(name: name);

                await serviceBusBroker.SendMessageAsync(
                    sender: sender,
                    message: message);
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

    private ServiceBusSender GetOrCreateSender(string name)
    {
        lock (senders)
        {
            if (!senders.TryGetValue(key: name, value: out ServiceBusSender sender))
            {
                sender = serviceBusBroker.CreateSender(name: name);
                senders[name] = sender;
            }

            return sender;
        }
    }

    private ServiceBusProcessor GetOrCreateProcessor(string name)
    {
        lock (processors)
        {
            if (!processors.TryGetValue(
                key: name,
                value: out ServiceBusProcessor processor))
            {
                processor = serviceBusBroker.CreateProcessor(name: name);
                processors[name] = processor;
            }

            return processor;
        }
    }

    private void StartProcessor(
        string name,
        ServiceBusProcessor processor)
    {
        lock (processors)
        {
            if (startedProcessors.Contains(item: name))
            {
                return;
            }

            startedProcessors.Add(item: name);
        }

        serviceBusBroker
            .StartProcessorAsync(processor: processor)
            .GetAwaiter()
            .GetResult();
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

    Task HandleServiceBusError(string name, ProcessErrorEventArgs problemDetails)
    {
        log.LogError(
            exception: problemDetails.Exception,
            message: "Exception thrown whilst listening to {Name} event:\n{Message}\n{StackTrace}",
            args: [name, problemDetails.Exception.Message, problemDetails.Exception.StackTrace]);

        return Task.CompletedTask;
    }
}