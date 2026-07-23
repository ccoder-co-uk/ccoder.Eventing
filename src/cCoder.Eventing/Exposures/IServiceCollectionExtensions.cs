// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing;

public static partial class IServiceCollectionExtensions
{
    public static void AddEventing(this IServiceCollection services) =>
        CreateServiceCollectionProcessingService()
            .AddConfiguredEventingConfiguration(
                services: services,
                newConfigure: static _ => { });

    public static void AddEventingWeb(
        this IServiceCollection services,
        Action<EventingConfiguration> configure = null) =>
        CreateServiceCollectionProcessingService()
            .AddConfiguredEventingConfiguration(
                services: services,
                newConfigure: configure ?? (static _ => { }));

    public static void AddEventingHostedServices(
        this IServiceCollection services,
        Action<EventingConfiguration> configure = null) =>
        CreateServiceCollectionProcessingService()
            .AddConfiguredEventingConfiguration(
                services: services,
                newConfigure: configure ?? (static _ => { }));

    public static void AddEventing(
        this IServiceCollection services,
        Action<EventingConfiguration> configure) =>
        CreateServiceCollectionProcessingService()
            .AddConfiguredEventingConfiguration(
                services: services,
                newConfigure: configure);

    public static void AddEventing(
        this IServiceCollection services,
        EventingConfiguration eventingConfiguration) =>
        CreateServiceCollectionProcessingService()
            .AddEventingConfiguration(
                services: services,
                newEventingConfiguration: eventingConfiguration ?? new EventingConfiguration());

    public static void AddEventProviders(
        this IServiceCollection services,
        params EventProvider[] eventProviders) =>
        CreateServiceCollectionProcessingService()
            .AddEventProviders(
                services: services,
                newEventProviders: eventProviders ?? []);

    public static void AddBulkEventProviders(
        this IServiceCollection services,
        params BulkEventProvider[] bulkEventProviders) =>
        CreateServiceCollectionProcessingService()
            .AddBulkEventProviders(
                services: services,
                newBulkEventProviders: bulkEventProviders ?? []);

    public static void AddEventingForType<T>(this IServiceCollection services) =>
        CreateServiceCollectionProcessingService()
            .AddEventingForType<T>(services: services);

    private static ServiceCollectionProcessingService CreateServiceCollectionProcessingService() =>
        new ServiceCollectionProcessingService();
}