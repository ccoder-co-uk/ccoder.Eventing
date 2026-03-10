using Azure.Messaging.ServiceBus;
using EventLibrary.AzureServiceBus.Brokers;
using EventLibrary.AzureServiceBus.Services.Foundations;
using EventLibrary.AzureServiceBus.Services.Processings;
using EventLibrary.Models;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.AzureServiceBus;

public static class IServiceCollectionExtensions
{
    public static void AddAzureServiceBusEventing(
        this IServiceCollection services,
        string serviceBusConnectionString,
        Func<IServiceProvider, string> getUserId)
    {
        services.AddEventing(getUserId);
        services.AddSingleton(_ => new ServiceBusClient(serviceBusConnectionString));

        services.AddTransient<Func<IEventAuthInfo>>(serviceProvider =>
            () => serviceProvider.GetRequiredService<IEventAuthInfo>());

        services.AddSingleton<IServiceBusBroker, ServiceBusBroker>();
        services.AddTransient<IServiceBusService, ServiceBusService>();
        services.AddTransient<IServiceBusProcessingService, ServiceBusProcessingService>();
        services.AddSingleton<IAzureServiceBusEventHub, AzureServiceBusEventHub>();

        services.AddSingleton<IEventHub>(serviceProvider =>
            serviceProvider.GetRequiredService<IAzureServiceBusEventHub>());
    }
}
