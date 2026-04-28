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
        AddEventing(services, static _ => { });

    public static void AddEventingWeb(
        this IServiceCollection services,
        Action<EventingConfiguration> configure = null) =>
        AddEventing(services, configure);

    public static void AddEventingHostedServices(
        this IServiceCollection services,
        Action<EventingConfiguration> configure = null) =>
        AddEventing(services, configure);

    public static void AddEventing(
        this IServiceCollection services,
        Action<EventingConfiguration> configure)
    {
        EventingConfiguration configuration = new();
        configure?.Invoke(configuration);
        RegisterEventing(services, configuration);
    }

    public static void AddEventing(
        this IServiceCollection services,
        EventingConfiguration eventingConfiguration)
    {
        EventingConfiguration configuration = eventingConfiguration ?? new EventingConfiguration();
        RegisterEventing(services, configuration);
    }

    private static void RegisterEventing(
        IServiceCollection services,
        EventingConfiguration configuration)
    {
        services.AddSingleton(configuration);
        services.AddEventProviders(configuration.EventProviders);
        services.AddBulkEventProviders(configuration.BulkEventProviders);
        services.AddScoped<IEventAuthorizationBroker, EventAuthorizationBroker>();
        services.AddTransient(serviceProvider =>
            serviceProvider.GetRequiredService<IEventAuthorizationBroker>().GetEventAuthInfo());

        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
        services.AddTransient<IEventAuthorizationService, EventAuthorizationService>();
        services.AddSingleton<IEventProviderService, EventProviderService>();
        services.AddSingleton<IEventServiceProviderService, EventServiceProviderService>();
        services.AddSingleton<IEventOrchestrationService, EventOrchestrationService>();
        services.AddSingleton<IEventHub>(serviceProvider =>
            new EventHub(serviceProvider.GetRequiredService<IEventOrchestrationService>()));
    }

    public static void AddEventProviders(
        this IServiceCollection services,
        params EventProvider[] eventProviders)
    {
        foreach (EventProvider eventProvider in eventProviders ?? [])
        {
            if (eventProvider is not null)
                services.AddSingleton(eventProvider);
        }
    }

    public static void AddBulkEventProviders(
        this IServiceCollection services,
        params BulkEventProvider[] bulkEventProviders)
    {
        foreach (BulkEventProvider bulkEventProvider in bulkEventProviders ?? [])
        {
            if (bulkEventProvider is not null)
                services.AddSingleton(bulkEventProvider);
        }
    }

    public static void AddEventingForType<T>(this IServiceCollection services)
    {
        services.AddSingleton<IEventBroker<T>, EventBroker<T>>();
        services.AddTransient<IEventService<T>, EventService<T>>();
        services.AddTransient<IEventProcessingService<T>, EventProcessingService<T>>();
    }
}
