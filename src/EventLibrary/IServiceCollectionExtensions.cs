using EventLibrary.Brokers;
using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using EventLibrary.Services.Processings;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary;

public static class IServiceCollectionExtensions
{
    public static void AddEventing(
        this IServiceCollection services,
        Func<IServiceProvider, string> getUserId)
    {
        services.AddScoped<IEventAuthorizationBroker, EventAuthorizationBroker>(
            serviceProvider => new EventAuthorizationBroker(() => getUserId(serviceProvider)));

        services.AddTransient(serviceProvider =>
            TryGetScopedAuthBroker(serviceProvider, getUserId).GetEventAuthInfo());

        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
        services.AddTransient<IEventAuthorizationService, EventAuthorizationService>();
        services.AddSingleton<IEventHub, EventHub>();
    }

    public static void AddEventingForType<T>(this IServiceCollection services)
    {
        services.AddTransient<IEventBroker<EventMessage<T>>, EventBroker<EventMessage<T>>>();
        services.AddTransient<IEventService<EventMessage<T>>, EventService<EventMessage<T>>>();
        services.AddTransient<IEventProcessingService<T>, EventProcessingService<T>>();
    }

    private static IEventAuthorizationBroker TryGetScopedAuthBroker(
        IServiceProvider services,
        Func<IServiceProvider, string> getUserId)
    {
        try
        {
            return services.GetService<IEventAuthorizationBroker>();
        }
        catch
        {
            return new EventAuthorizationBroker(() => getUserId(services));
        }
    }
}
