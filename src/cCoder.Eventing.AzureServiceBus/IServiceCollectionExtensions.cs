using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Foundations;
using cCoder.Eventing.AzureServiceBus.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus;

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
