namespace EventLibrary.AzureServiceBus.Models;

public interface IServiceBusEventAuthInfo
{
    string SSOUserId { get; set; }
}
