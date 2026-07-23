// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.AzureServiceBus;

public static partial class IServiceCollectionExtensions
{
    public static void AddAzureServiceBusEventingWeb(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure = null) =>
        CreateServiceCollectionProcessingService()
            .AddConfiguredAzureServiceBusEventingConfiguration(
                services: services,
                newConfigure: configure ?? (static _ => { }));

    public static void AddAzureServiceBusEventingHostedServices(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure = null) =>
        CreateServiceCollectionProcessingService()
            .AddConfiguredAzureServiceBusEventingConfiguration(
                services: services,
                newConfigure: configure ?? (static _ => { }));

    public static void AddAzureServiceBusEventing(
        this IServiceCollection services,
        Action<AzureServiceBusEventingConfiguration> configure) =>
        CreateServiceCollectionProcessingService()
            .AddConfiguredAzureServiceBusEventingConfiguration(
                services: services,
                newConfigure: configure);

    public static void AddAzureServiceBusEventing(
        this IServiceCollection services,
        string serviceBusConnectionString) =>
        CreateServiceCollectionProcessingService()
            .AddAzureServiceBusEventingConnection(
                services: services,
                serviceBusConnectionString: serviceBusConnectionString);

    private static ServiceCollectionProcessingService CreateServiceCollectionProcessingService() =>
        new ServiceCollectionProcessingService();
}