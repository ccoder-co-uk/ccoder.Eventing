// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using cCoder.Eventing.Models;
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
        services.AddBrokers(configuration: configuration);
        services.AddFoundations();
        services.AddProcessings();
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
        services.AddBrokers(configuration: configuration);
        services.AddFoundations();
        services.AddProcessings();
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

    private static IServiceCollection AddBrokers(
        this IServiceCollection services,
        HttpEventingOptions configuration)
    {
        services.TryAddSingleton<ILoggingBroker, LoggingBroker>();
        services.TryAddSingleton<IHttpEventBroker>(
            implementationFactory: serviceProvider =>
                new HttpEventBroker(
                    httpClientFactory: serviceProvider
                        .GetRequiredService<IHttpClientFactory>(),
                    configuration: new HttpEventBrokerConfiguration
                    {
                        HubUrl = configuration.HubUrl,
                        JsonSerializerOptions =
                            configuration.JsonSerializerOptions,
                        MaxConcurrency = configuration.MaxConcurrency,
                        EventProviderConfigurations =
                            serviceProvider
                                .GetServices<EventProvider>()
                                .Select(selector: eventProvider =>
                                    new HttpEventProviderBrokerConfiguration
                                    {
                                        CanReceive = eventProvider.CanReceive,
                                        DataType = eventProvider.DataType,
                                        ReceiveAsync = eventProvider.ReceiveAsync,
                                        JsonSerializerOptions = configuration
                                            .JsonSerializerOptions
                                    })
                                .ToArray()
                    }));

        return services;
    }

    private static IServiceCollection AddFoundations(this IServiceCollection services)
    {
        services.TryAddTransient<IHttpEventService, HttpEventService>();

        return services;
    }

    private static IServiceCollection AddProcessings(this IServiceCollection services)
    {
        services.TryAddTransient<
            IHttpEventProcessingService,
            HttpEventProcessingService>();
        return services;
    }

    private static IServiceCollection AddExposures(this IServiceCollection services)
    {
        services.TryAddSingleton<IHttpEventHub>(
            implementationFactory: serviceProvider =>
                new HttpEventHub(
                    httpEventProcessingService: serviceProvider
                        .GetRequiredService<IHttpEventProcessingService>()));

        services.AddHostedService<HttpEventDispatcherHostedService>();

        return services;
    }
}