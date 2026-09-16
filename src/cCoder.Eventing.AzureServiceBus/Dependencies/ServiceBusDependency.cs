// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;

namespace cCoder.Eventing.AzureServiceBus.Dependencies;

internal sealed class ServiceBusDependency : IAsyncDisposable
{
    private readonly ServiceBusClient client;
    private readonly int maxConcurrency;

    private readonly Dictionary<string, ServiceBusSender> senders = [];
    private readonly Dictionary<string, ServiceBusProcessor> processors = [];

    internal ServiceBusDependency(
        string connectionString,
        int maxConcurrency)
        : this(
            maxConcurrency: maxConcurrency,
            client: new ServiceBusClient(
                connectionString: connectionString))
    { }

    internal ServiceBusDependency(
        int maxConcurrency,
        ServiceBusClient client)
    {
        this.maxConcurrency = maxConcurrency;
        this.client = client;
    }

    internal async ValueTask SendAsync(
        string name,
        BinaryData body,
        string messageId)
    {
        ServiceBusSender sender = GetOrCreateSender(name: name);
        ServiceBusMessage message = new()
        {
            Body = body,
            MessageId = messageId
        };

        await sender.SendMessageAsync(message: message);
    }

    internal void Listen(
        string name,
        Func<BinaryData, ValueTask> handler,
        Func<Exception, Task> errorHandler)
    {
        ServiceBusProcessor processor = GetOrCreateProcessor(name: name);

        processor.ProcessMessageAsync += async messageDetails =>
        {
            await handler(messageDetails.Message.Body);
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
                            val2: maxConcurrency)
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