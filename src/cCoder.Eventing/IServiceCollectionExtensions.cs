// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Brokers.Loggings;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Orchestrations;
using cCoder.Eventing.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing;

public static class IServiceCollectionExtensions
{
    public static void AddEventingWeb(
        this IServiceCollection services,
        Action<EventingConfiguration> configure = null)
    {
        EventingConfiguration configuration = new();
        configure?.Invoke(obj: configuration);
        services.AddEventingWeb(configuration: configuration);
    }

    public static void AddEventingWeb(
        this IServiceCollection services,
        EventingConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
    }

    public static void AddEventingHostedServices(
        this IServiceCollection services,
        Action<EventingConfiguration> configure = null)
    {
        EventingConfiguration configuration = new();
        configure?.Invoke(obj: configuration);
        services.AddEventingHostedServices(configuration: configuration);
    }

    public static void AddEventingHostedServices(
        this IServiceCollection services,
        EventingConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
    }

    public static void AddEventing(this IServiceCollection services)
    {
        EventingConfiguration configuration = new();

        services.AddConfiguration(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
    }

    public static void AddEventing(
        this IServiceCollection services,
        Action<EventingConfiguration> configure)
    {
        EventingConfiguration configuration = new();
        configure?.Invoke(obj: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
    }

    public static void AddEventing(
        this IServiceCollection services,
        EventingConfiguration eventingConfiguration)
    {
        ArgumentNullException.ThrowIfNull(argument: eventingConfiguration);

        services.AddConfiguration(configuration: eventingConfiguration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
    }

    public static void AddEventProviders(
        this IServiceCollection services,
        params EventProvider[] eventProviders) =>
        services.AddExposures(eventProviders: eventProviders);

    public static void AddBulkEventProviders(
        this IServiceCollection services,
        params BulkEventProvider[] bulkEventProviders) =>
        services.AddExposures(bulkEventProviders: bulkEventProviders);

    public static void AddEventingForType<T>(this IServiceCollection services)
    {
        services.AddBrokersForType<T>();
        services.AddFoundationsForType<T>();
        services.AddProcessingsForType<T>();
    }

    private static void AddExposures(
        this IServiceCollection services,
        EventProvider[] eventProviders)
    {
        foreach (EventProvider eventProvider in eventProviders ?? [])
        {
            if (eventProvider is not null)
            {
                services.AddSingleton(implementationInstance: eventProvider);
            }
        }
    }

    private static void AddExposures(
        this IServiceCollection services,
        BulkEventProvider[] bulkEventProviders)
    {
        foreach (BulkEventProvider bulkEventProvider in bulkEventProviders ?? [])
        {
            if (bulkEventProvider is not null)
            {
                services.AddSingleton(implementationInstance: bulkEventProvider);
            }
        }
    }

    private static void AddBrokersForType<T>(this IServiceCollection services)
    {
    }

    private static void AddFoundationsForType<T>(this IServiceCollection services)
    {
        services.AddSingleton<IEventService<T>, EventService<T>>();
    }

    private static void AddProcessingsForType<T>(this IServiceCollection services)
    {
        services.AddTransient<IEventProcessingService<T>, EventProcessingService<T>>();
    }

    private static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        EventingConfiguration configuration)
    {
        services.AddSingleton(implementationInstance: configuration);
        services.AddExposures(eventProviders: configuration.EventProviders ?? []);
        services.AddExposures(
            bulkEventProviders: configuration.BulkEventProviders ?? []);

        return services;
    }

    private static IServiceCollection AddBrokers(this IServiceCollection services)
    {
        services.AddSingleton<ILoggingBroker, LoggingBroker>();
        services.AddScoped<IEventAuthorizationBroker, EventAuthorizationBroker>();
        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();

        return services;
    }

    private static IServiceCollection AddFoundations(this IServiceCollection services)
    {
        services.AddTransient<IEventAuthorizationService, EventAuthorizationService>();
        services.AddSingleton<IEventProviderService, EventProviderService>();
        services.AddSingleton<IEventServiceProviderService, EventServiceProviderService>();

        return services;
    }

    private static IServiceCollection AddProcessings(this IServiceCollection services) =>
        services;

    private static IServiceCollection AddOrchestrations(
        this IServiceCollection services)
    {
        services.AddSingleton<IEventOrchestrationService, EventOrchestrationService>();

        return services;
    }

    private static IServiceCollection AddExposures(this IServiceCollection services)
    {
        services.AddTransient(implementationFactory: serviceProvider =>
            serviceProvider
                .GetRequiredService<IEventAuthorizationBroker>()
                .GetEventAuthInfo());

        services.AddSingleton<IEventHub>(implementationFactory: serviceProvider =>
            new EventHub(
                serviceProvider.GetRequiredService<IEventOrchestrationService>()));

        return services;
    }
}