using Azure.Messaging.ServiceBus;
using EventLibrary.AzureServiceBus.Brokers;
using EventLibrary.AzureServiceBus.Models;
using EventLibrary.AzureServiceBus.Services.Foundations;
using EventLibrary.AzureServiceBus.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary.AzureServiceBus;

public static class IServiceCollectionExtensions
{
    public static void AddAzureServiceBusEventing(
        this IServiceCollection services,
        string serviceBusConnectionString)
    {
        services.AddSingleton(_ => new ServiceBusClient(serviceBusConnectionString));

        services.AddScoped<IServiceBusEventAuthorizationBroker, ServiceBusEventAuthorizationBroker>();
        services.AddTransient(serviceProvider =>
            serviceProvider.GetRequiredService<IServiceBusEventAuthorizationBroker>().GetEventAuthInfo());

        services.AddTransient<Func<IServiceBusEventAuthInfo>>(serviceProvider =>
            () => serviceProvider.GetRequiredService<IServiceBusEventAuthInfo>());

        services.AddSingleton<IServiceBusBroker, ServiceBusBroker>();
        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
        services.AddTransient<IServiceBusService, ServiceBusService>();
        services.AddTransient<IServiceBusProcessingService, ServiceBusProcessingService>();
        services.AddSingleton<IAzureServiceBusEventHub>(serviceProvider =>
            new AzureServiceBusEventHub(serviceProvider.GetRequiredService<IServiceBusProcessingService>()));
    }
}
