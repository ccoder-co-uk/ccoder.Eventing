namespace cCoder.Eventing.AzureServiceBus.Models;

public class ServiceBusEventAuthInfo : IServiceBusEventAuthInfo
{
    public string SSOUserId { get; set; }
}
