using Azure.Messaging.ServiceBus;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal class ServiceBusBroker : IServiceBusBroker
{
    private readonly ServiceBusClient serviceBusClient;
    private readonly IDictionary<string, ServiceBusSender> senders;
    private readonly IDictionary<string, ServiceBusProcessor> receivers;
    private readonly ISet<string> startedReceivers;

    public ServiceBusBroker(ServiceBusClient serviceBusClient)
    {
        this.serviceBusClient = serviceBusClient;
        senders = new Dictionary<string, ServiceBusSender>();
        receivers = new Dictionary<string, ServiceBusProcessor>();
        startedReceivers = new HashSet<string>();
    }

    public async ValueTask SendMessageAsync(string name, ServiceBusMessage message)
    {
        ServiceBusSender sender = senders.ContainsKey(name)
            ? senders[name]
            : CreateNewSender(name);

        await sender.SendMessageAsync(message);
    }

    public ServiceBusProcessor CreateProcessor(string name)
    {
        ServiceBusProcessor receiver = receivers.ContainsKey(name)
            ? receivers[name]
            : CreateServiceBusProcessor(name);

        return receiver;
    }

    public async ValueTask StartProcessorAsync(string name)
    {
        ServiceBusProcessor receiver = CreateProcessor(name);

        lock (receivers)
        {
            if (startedReceivers.Contains(name))
            {
                return;
            }

            startedReceivers.Add(name);
        }

        await receiver.StartProcessingAsync();
    }

    private ServiceBusProcessor CreateServiceBusProcessor(string name)
    {
        lock (receivers)
        {
            if (receivers.ContainsKey(name))
            {
                return receivers[name];
            }

            receivers[name] = serviceBusClient.CreateProcessor(name);
            return receivers[name];
        }
    }

    private ServiceBusSender CreateNewSender(string name)
    {
        lock (senders)
        {
            senders[name] = serviceBusClient.CreateSender(name);
            return senders[name];
        }
    }

    ~ServiceBusBroker()
    {
        foreach (KeyValuePair<string, ServiceBusSender> sender in senders)
        {
            sender.Value.DisposeAsync().AsTask().Wait();
        }
    }
}
