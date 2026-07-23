// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Dependencies;
using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus.Services.Processings;

internal sealed partial class ServiceCollectionProcessingService
    : IServiceCollectionProcessingService
{
    public void AddConfiguredAzureServiceBusEventingConfiguration(
        IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> newConfigure) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [services, newConfigure]);

            AzureServiceBusEventingConfiguration configuration = new();
            newConfigure.Invoke(obj: configuration);

            RegisterAzureServiceBusEventing(
                services: services,
                configuration: configuration);
        });

    public void AddAzureServiceBusEventingConnection(
        IServiceCollection services,
        string serviceBusConnectionString) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [services, serviceBusConnectionString]);

            AzureServiceBusEventingConfiguration configuration = new()
            {
                ConnectionString = serviceBusConnectionString
            };

            RegisterAzureServiceBusEventing(
                services: services,
                configuration: configuration);
        });

    private static void RegisterAzureServiceBusEventing(
        IServiceCollection services,
        AzureServiceBusEventingConfiguration configuration)
    {
        services.AddSingleton(implementationInstance: configuration);

        services.AddSingleton(implementationFactory: _ =>
            new ServiceBusClient(configuration.ConnectionString));

        services.AddScoped<IServiceBusEventAuthorizationBroker, ServiceBusEventAuthorizationBroker>();

        services.AddTransient(implementationFactory: serviceProvider =>
            serviceProvider
                .GetRequiredService<IServiceBusEventAuthorizationBroker>()
                .GetEventAuthInfo());

        services.AddTransient<Func<IServiceBusEventAuthInfo>>(implementationFactory: serviceProvider =>
            () => serviceProvider.GetRequiredService<IServiceBusEventAuthInfo>());

        services.AddSingleton<IServiceBusBroker, ServiceBusBroker>();
        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
        services.AddTransient<IServiceBusService, ServiceBusService>();
        services.AddTransient<IServiceBusProcessingService, ServiceBusProcessingService>();

        services.AddSingleton<IAzureServiceBusEventHub>(implementationFactory: serviceProvider =>
            new AzureServiceBusEventHub(
                serviceProvider.GetRequiredService<IServiceBusProcessingService>()));
    }
}