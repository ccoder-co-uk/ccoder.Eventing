// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Dependencies;

internal sealed class ServiceBusDependency(
    AzureServiceBusEventingConfiguration configuration) :
    IAsyncDisposable
{
    private readonly ServiceBusClient client =
        new(connectionString: configuration.ConnectionString);

    private readonly Dictionary<string, ServiceBusSender> senders = [];
    private readonly Dictionary<string, ServiceBusProcessor> processors = [];

    internal async ValueTask SendAsync<T>(
        string name,
        ServiceBusEventMessage<T> eventMessage)
    {
        ServiceBusSender sender = GetOrCreateSender(name: name);
        ServiceBusMessage message = new()
        {
            Body = new BinaryData(eventMessage),
            MessageId = $"{eventMessage.AuthInfo.SSOUserId}_{typeof(T).Name}_{Guid.NewGuid()}"
        };

        await sender.SendMessageAsync(message: message);
    }

    internal void Listen<T>(
        string name,
        Func<ServiceBusEventMessage<T>, ValueTask> handler,
        Func<Exception, Task> errorHandler)
    {
        ServiceBusProcessor processor = GetOrCreateProcessor(name: name);

        processor.ProcessMessageAsync += async messageDetails =>
        {
            ServiceBusEventMessage<T> message = messageDetails
                .Message
                .Body
                .ToObjectFromJson<ServiceBusEventMessage<T>>();

            await handler(message);
        };

        processor.ProcessErrorAsync += problemDetails =>
            errorHandler(problemDetails.Exception);

        processor.StartProcessingAsync().GetAwaiter().GetResult();
    }

    private ServiceBusSender GetOrCreateSender(string name)
    {
        lock (senders)
        {
            if (!senders.TryGetValue(key: name, value: out ServiceBusSender sender))
            {
                sender = client.CreateSender(queueOrTopicName: name);
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
                processor = client.CreateProcessor(
                    queueName: name,
                    options: new ServiceBusProcessorOptions
                    {
                        MaxConcurrentCalls = Math.Max(
                            val1: 1,
                            val2: configuration.MaxConcurrency)
                    });

                processors[name] = processor;
            }

            return processor;
        }
    }

    public async ValueTask DisposeAsync()
    {
        foreach (ServiceBusProcessor processor in processors.Values)
        {
            await processor.DisposeAsync();
        }

        foreach (ServiceBusSender sender in senders.Values)
        {
            await sender.DisposeAsync();
        }

        await client.DisposeAsync();
    }
}