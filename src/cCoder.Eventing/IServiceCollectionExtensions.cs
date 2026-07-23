// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Orchestrations;
using cCoder.Eventing.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing;

public static partial class IServiceCollectionExtensions
{
    public static void AddEventing(this IServiceCollection services) =>
        AddEventing(services:services, configure:static _ => { });

    public static void AddEventingWeb(
        this IServiceCollection services,
        Action<EventingConfiguration> configure = null) =>
        AddEventing(services:services, configure:configure);

    public static void AddEventingHostedServices(
        this IServiceCollection services,
        Action<EventingConfiguration> configure = null) =>
        AddEventing(services:services, configure:configure);

    public static void AddEventing(
        this IServiceCollection services,
        Action<EventingConfiguration> configure)
    {
        EventingConfiguration configuration = new();
        configure?.Invoke(obj:configuration);
        RegisterEventing(services:services, configuration:configuration);
    }

    public static void AddEventing(
        this IServiceCollection services,
        EventingConfiguration eventingConfiguration)
    {
        EventingConfiguration configuration = eventingConfiguration ?? new EventingConfiguration();
        RegisterEventing(services:services, configuration:configuration);
    }

    private static void RegisterEventing(
        IServiceCollection services,
        EventingConfiguration configuration)
    {
        services.AddSingleton(implementationInstance:configuration);
        services.AddEventProviders(eventProviders:configuration.EventProviders);
        services.AddBulkEventProviders(bulkEventProviders:configuration.BulkEventProviders);
        services.AddScoped<IEventAuthorizationBroker, EventAuthorizationBroker>();
        services.AddTransient(implementationFactory:serviceProvider =>
            serviceProvider.GetRequiredService<IEventAuthorizationBroker>().GetEventAuthInfo());

        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
        services.AddTransient<IEventAuthorizationService, EventAuthorizationService>();
        services.AddSingleton<IEventProviderService, EventProviderService>();
        services.AddSingleton<IEventServiceProviderService, EventServiceProviderService>();
        services.AddSingleton<IEventOrchestrationService, EventOrchestrationService>();
        services.AddSingleton<IEventHub>(implementationFactory:serviceProvider =>
            new EventHub(serviceProvider.GetRequiredService<IEventOrchestrationService>()));
    }

    public static void AddEventProviders(
        this IServiceCollection services,
        params EventProvider[] eventProviders)
    {
        foreach (EventProvider eventProvider in eventProviders ?? [])
        {
            if (eventProvider is not null)
                services.AddSingleton(implementationInstance:eventProvider);
        }
    }

    public static void AddBulkEventProviders(
        this IServiceCollection services,
        params BulkEventProvider[] bulkEventProviders)
    {
        foreach (BulkEventProvider bulkEventProvider in bulkEventProviders ?? [])
        {
            if (bulkEventProvider is not null)
                services.AddSingleton(implementationInstance:bulkEventProvider);
        }
    }

    public static void AddEventingForType<T>(this IServiceCollection services)
    {
        services.AddSingleton<IEventBroker<T>, EventBroker<T>>();
        services.AddTransient<IEventService<T>, EventService<T>>();
        services.AddTransient<IEventProcessingService<T>, EventProcessingService<T>>();
    }
}