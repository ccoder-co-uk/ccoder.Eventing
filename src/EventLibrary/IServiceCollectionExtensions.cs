using EventLibrary.Brokers;
using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using EventLibrary.Services.Orchestrations;
using EventLibrary.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary;

public static class IServiceCollectionExtensions
{
    public static void AddEventing(this IServiceCollection services)
    {
        services.AddScoped<IEventAuthorizationBroker, EventAuthorizationBroker>();
        services.AddTransient(serviceProvider =>
            serviceProvider.GetRequiredService<IEventAuthorizationBroker>().GetEventAuthInfo());

        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
        services.AddTransient<IEventAuthorizationService, EventAuthorizationService>();
        services.AddSingleton<IEventServiceProviderService, EventServiceProviderService>();
        services.AddSingleton<IEventOrchestrationService, EventOrchestrationService>();
        services.AddSingleton<IEventHub>(serviceProvider =>
            new EventHub(serviceProvider.GetRequiredService<IEventOrchestrationService>()));
    }

    public static void AddEventingForType<T>(this IServiceCollection services)
    {
        services.AddSingleton<IEventBroker<T>, EventBroker<T>>();
        services.AddTransient<IEventService<T>, EventService<T>>();
        services.AddTransient<IEventProcessingService<T>, EventProcessingService<T>>();
    }
}
