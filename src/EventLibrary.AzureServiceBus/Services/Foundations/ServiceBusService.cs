using Azure.Messaging.ServiceBus;
using EventLibrary.AzureServiceBus.Brokers;
using EventLibrary.Models;
using Microsoft.Extensions.Logging;

namespace EventLibrary.AzureServiceBus.Services.Foundations;

public class ServiceBusService : IServiceBusService
{
    private readonly IServiceBusBroker serviceBusBroker;
    private readonly ILogger<ServiceBusService> log;

    public ServiceBusService(
        IServiceBusBroker serviceBusBroker,
        ILogger<ServiceBusService> log)
    {
        this.serviceBusBroker = serviceBusBroker;
        this.log = log;
    }

    public async ValueTask RaiseEventAsync<T>(string name, EventMessage<T> eventMessage)
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

            throw new InvalidOperationException(
                "Could not raise event due to exception, see inner exception for details.",
                ex);
        }
    }
}
