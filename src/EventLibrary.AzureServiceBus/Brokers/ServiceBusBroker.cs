using Azure.Messaging.ServiceBus;
using EventLibrary.AzureServiceBus.Brokers.Interfaces;

namespace EventLibrary.AzureServiceBus.Brokers;

public class ServiceBusBroker : IServiceBusBroker
{
    private readonly ServiceBusClient serviceBusClient;
    private readonly IDictionary<string, ServiceBusSender> senders;

    public ServiceBusBroker(ServiceBusClient serviceBusClient)
    {
        this.serviceBusClient = serviceBusClient;
        senders = new Dictionary<string, ServiceBusSender>();
    }

    public async ValueTask SendMessageAsync(string name, ServiceBusMessage message)
    {
        ServiceBusSender sender = senders.ContainsKey(name)
            ? senders[name]
            : CreateNewSender(name);

        await sender.SendMessageAsync(message);
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
