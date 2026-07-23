// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Foundations;
using cCoder.Eventing.AzureServiceBus.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus;

public static partial class IServiceCollectionExtensions
{
    public static void AddAzureServiceBusEventingWeb(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure = null) =>
        AddAzureServiceBusEventing(services, configure);

    public static void AddAzureServiceBusEventingHostedServices(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure = null) =>
        AddAzureServiceBusEventing(services, configure);

    public static void AddAzureServiceBusEventing(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure)
    {
        AzureServiceBusEventingConfiguration configuration = new();
        configure?.Invoke(configuration);
        RegisterAzureServiceBusEventing(services, configuration);
    }

    public static void AddAzureServiceBusEventing(
        this IServiceCollection services,
        string serviceBusConnectionString)
    {
        AddAzureServiceBusEventing(
            services,
            configuration => configuration.ConnectionString = serviceBusConnectionString);
    }

    private static void RegisterAzureServiceBusEventing(
        IServiceCollection services,
        AzureServiceBusEventingConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddSingleton(_ => new ServiceBusClient(configuration.ConnectionString));

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