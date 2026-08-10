// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Dependencies;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text.Json;

namespace cCoder.Eventing.Http;

public static class IServiceCollectionExtensions
{
    public static void AddHttpEventingWeb(
        this IServiceCollection services,
        Action<HttpEventingOptions> configure = null)
    {
        HttpEventingOptions configuration = new()
        {
            MaxConcurrency = 1,
            JsonSerializerOptions =
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
        };
        configure?.Invoke(obj: configuration);
        services.AddHttpEventingWeb(configuration: configuration);
    }

    public static void AddHttpEventingWeb(
        this IServiceCollection services,
        HttpEventingOptions configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
    }

    public static void AddHttpEventingHostedServices(
        this IServiceCollection services,
        Action<HttpEventingOptions> configure = null)
    {
        HttpEventingOptions configuration = new()
        {
            MaxConcurrency = 1,
            JsonSerializerOptions =
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
        };
        configure?.Invoke(obj: configuration);
        services.AddHttpEventingHostedServices(configuration: configuration);
    }

    public static void AddHttpEventingHostedServices(
        this IServiceCollection services,
        HttpEventingOptions configuration)
    {
        ArgumentNullException.ThrowIfNull(argument: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
        IMvcBuilder mvcBuilder = services.AddControllers();
        mvcBuilder.AddHttpEventingControllers();
    }

    private static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        HttpEventingOptions configuration)
    {
        services.TryAddSingleton(instance: configuration);
        services.AddHttpClient(name: HttpEventingOptions.HttpClientName);

        return services;
    }

    private static IServiceCollection AddBrokers(this IServiceCollection services)
    {
        services.TryAddSingleton<ILoggingBroker, LoggingBroker>();
        services.TryAddSingleton<IHttpEventQueue, HttpEventQueue>();
        services.TryAddSingleton<IHttpEventHandlerRegistry, HttpEventHandlerRegistry>();
        services.TryAddSingleton<IHttpEventBroker, HttpEventBroker>();
        services.TryAddSingleton<IHttpEventDispatcher, HttpEventDispatcher>();

        return services;
    }

    private static IServiceCollection AddFoundations(this IServiceCollection services)
    {
        services.TryAddTransient<
            IHttpEventService,
            HttpEventServiceDependency>();

        return services;
    }

    private static IServiceCollection AddProcessings(this IServiceCollection services)
    {
        services.TryAddTransient<
            IHttpEventProcessingService,
            HttpEventProcessingServiceDependency>();

        return services;
    }

    private static IServiceCollection AddOrchestrations(
        this IServiceCollection services) =>
        services;

    private static IServiceCollection AddExposures(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpEventHub>(
            implementationFactory: serviceProvider =>
                new HttpEventHub(
                    serviceProvider
                        .GetRequiredService<IHttpEventProcessingService>()));

        services.AddHostedService<HttpEventDispatcherHostedService>();

        return services;
    }
}