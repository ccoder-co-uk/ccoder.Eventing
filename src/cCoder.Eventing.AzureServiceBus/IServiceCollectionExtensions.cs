// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Dependencies;
using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Foundations;
using cCoder.Eventing.AzureServiceBus.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus;

public static class IServiceCollectionExtensions
{
    public static void AddAzureServiceBusEventingWeb(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure = null)
    {
        AzureServiceBusEventingConfiguration configuration = new();
        configure?.Invoke(obj: configuration);
        services.AddAzureServiceBusEventingWeb(configuration: configuration);
    }

    public static void AddAzureServiceBusEventingWeb(
        this IServiceCollection services,
        AzureServiceBusEventingConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddDependencies(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddExposures();
    }

    public static void AddAzureServiceBusEventingHostedServices(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure = null)
    {
        AzureServiceBusEventingConfiguration configuration = new();
        configure?.Invoke(obj: configuration);
        services.AddAzureServiceBusEventingHostedServices(
            configuration: configuration);
    }

    public static void AddAzureServiceBusEventingHostedServices(
        this IServiceCollection services,
        AzureServiceBusEventingConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddDependencies(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddExposures();
    }

    public static void AddAzureServiceBusEventing(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure)
    {
        AzureServiceBusEventingConfiguration configuration = new();
        configure?.Invoke(obj: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddDependencies(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddExposures();
    }

    public static void AddAzureServiceBusEventing(
        this IServiceCollection services,
        string serviceBusConnectionString)
    {
        AzureServiceBusEventingConfiguration configuration = new()
        {
            ConnectionString = serviceBusConnectionString
        };

        services.AddConfiguration(configuration: configuration);
        services.AddDependencies(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddExposures();
    }

    private static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        AzureServiceBusEventingConfiguration configuration)
    {
        services.AddSingleton(implementationInstance: configuration);

        return services;
    }

    private static IServiceCollection AddDependencies(
        this IServiceCollection services,
        AzureServiceBusEventingConfiguration configuration)
    {
        services.AddSingleton(implementationFactory: _ =>
            new ServiceBusClient(configuration.ConnectionString));

        return services;
    }

    private static IServiceCollection AddBrokers(this IServiceCollection services)
    {
        services.AddScoped<
            IServiceBusEventAuthorizationBroker,
            ServiceBusEventAuthorizationBroker>();
        services.AddSingleton<IServiceBusBroker, ServiceBusBroker>();
        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();

        return services;
    }

    private static IServiceCollection AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IServiceBusService, ServiceBusService>();

        return services;
    }

    private static IServiceCollection AddProcessings(this IServiceCollection services)
    {
        services.AddTransient<IServiceBusProcessingService, ServiceBusProcessingService>();

        return services;
    }

    private static IServiceCollection AddExposures(this IServiceCollection services)
    {
        services.AddTransient(implementationFactory: serviceProvider =>
            serviceProvider
                .GetRequiredService<IServiceBusEventAuthorizationBroker>()
                .GetEventAuthInfo());

        services.AddTransient<Func<IServiceBusEventAuthInfo>>(
            implementationFactory: serviceProvider =>
                () => serviceProvider.GetRequiredService<IServiceBusEventAuthInfo>());

        services.AddSingleton<IAzureServiceBusEventHub>(
            implementationFactory: serviceProvider =>
                new AzureServiceBusEventHub(
                    serviceProvider.GetRequiredService<IServiceBusProcessingService>()));

        return services;
    }
}