// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Dependencies;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Orchestrations;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Services.Processings;

internal sealed partial class ServiceCollectionProcessingService
    : IServiceCollectionProcessingService
{
    public void AddConfiguredEventingConfiguration(
        IServiceCollection services,
        Action<EventingConfiguration> newConfigure) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [services, newConfigure]);

            EventingConfiguration configuration = new();
            newConfigure.Invoke(obj: configuration);

            RegisterEventing(
                services: services,
                configuration: configuration);
        });

    public void AddEventingConfiguration(
        IServiceCollection services,
        EventingConfiguration newEventingConfiguration) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [services, newEventingConfiguration]);

            RegisterEventing(
                services: services,
                configuration: newEventingConfiguration);
        });

    public void AddEventProviders(
        IServiceCollection services,
        EventProvider[] newEventProviders) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [services, newEventProviders]);

            AddEventProvidersInternal(
                services: services,
                eventProviders: newEventProviders);
        });

    public void AddBulkEventProviders(
        IServiceCollection services,
        BulkEventProvider[] newBulkEventProviders) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [services, newBulkEventProviders]);

            AddBulkEventProvidersInternal(
                services: services,
                bulkEventProviders: newBulkEventProviders);
        });

    public void AddEventingForType<T>(IServiceCollection services) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: services);
            services.AddSingleton<IEventBroker<T>, EventBroker<T>>();
            services.AddTransient<IEventService<T>, EventService<T>>();
            services.AddTransient<IEventProcessingService<T>, EventProcessingService<T>>();
        });

    private static void RegisterEventing(
        IServiceCollection services,
        EventingConfiguration configuration)
    {
        services.AddSingleton(implementationInstance: configuration);

        AddEventProvidersInternal(
            services: services,
            eventProviders: configuration.EventProviders ?? []);

        AddBulkEventProvidersInternal(
            services: services,
            bulkEventProviders: configuration.BulkEventProviders ?? []);

        services.AddScoped<IEventAuthorizationBroker, EventAuthorizationBroker>();

        services.AddTransient(implementationFactory: serviceProvider =>
            serviceProvider
                .GetRequiredService<IEventAuthorizationBroker>()
                .GetEventAuthInfo());

        services.AddTransient<IServiceProviderBroker, ServiceProviderBroker>();
        services.AddTransient<IEventAuthorizationService, EventAuthorizationService>();
        services.AddSingleton<IEventProviderService, EventProviderService>();
        services.AddSingleton<IEventServiceProviderService, EventServiceProviderService>();
        services.AddSingleton<IEventOrchestrationService, EventOrchestrationService>();

        services.AddSingleton<IEventHub>(implementationFactory: serviceProvider =>
            new EventHub(
                serviceProvider.GetRequiredService<IEventOrchestrationService>()));
    }

    private static void AddEventProvidersInternal(
        IServiceCollection services,
        EventProvider[] eventProviders)
    {
        foreach (EventProvider eventProvider in eventProviders)
        {
            if (eventProvider is not null)
            {
                services.AddSingleton(implementationInstance: eventProvider);
            }
        }
    }

    private static void AddBulkEventProvidersInternal(
        IServiceCollection services,
        BulkEventProvider[] bulkEventProviders)
    {
        foreach (BulkEventProvider bulkEventProvider in bulkEventProviders)
        {
            if (bulkEventProvider is not null)
            {
                services.AddSingleton(implementationInstance: bulkEventProvider);
            }
        }
    }
}