using EventLibrary.Brokers;
using EventLibrary.Brokers.Interfaces;
using EventLibrary.Objects;
using EventLibrary.Services.Foundation;
using EventLibrary.Services.Foundation.Interfaces;
using EventLibrary.Services.Processing;
using EventLibrary.Services.Processing.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace EventLibrary
{
    public static class IServiceCollectionExtensions
    {
        public static void AddEventing(this IServiceCollection services, Func<IServiceProvider, string> getUserId)
        {
            services.AddScoped<IEventAuthorizationBroker, EventAuthorizationBroker>(
                serviceProvider => new EventAuthorizationBroker(() => getUserId(serviceProvider)));

            services.AddTransient(services => TryGetScopedAuthBroker(services, getUserId).GetEventAuthInfo());

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


        static IEventAuthorizationBroker TryGetScopedAuthBroker(
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
}