// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace cCoder.Eventing.Http;

public static partial class IServiceCollectionExtensions
{
    public static void AddHttpEventingWeb(
        this IServiceCollection services,
        Action<HttpEventingOptions> configure = null) =>
        AddHttpEventing(services:services, configure:configure);

    public static void AddHttpEventingHostedServices(
        this IServiceCollection services,
        Action<HttpEventingOptions> configure = null)
    {
        AddHttpEventing(services:services, configure:configure);

        services.AddControllers()
            .AddHttpEventingControllers();
    }

    public static void AddHttpEventing(
        this IServiceCollection services,
        Action<HttpEventingOptions> configure = null)
    {
        HttpEventingOptions options = new();
        configure?.Invoke(obj:options);
        RegisterHttpEventing(services:services, options:options);
    }

    private static void RegisterHttpEventing(
        IServiceCollection services,
        HttpEventingOptions options)
    {
        services.TryAddSingleton(instance:options);
        services.AddHttpClient(name:HttpEventingOptions.HttpClientName);

        services.TryAddSingleton<IHttpEventQueue, HttpEventQueue>();
        services.TryAddSingleton<IHttpEventHandlerRegistry, HttpEventHandlerRegistry>();
        services.TryAddSingleton<IHttpEventBroker, HttpEventBroker>();
        services.TryAddSingleton<IHttpEventDispatcher, HttpEventDispatcher>();
        services.TryAddTransient<IHttpEventService, HttpEventService>();
        services.TryAddTransient<IHttpEventProcessingService, HttpEventProcessingService>();

        services.TryAddSingleton<IHttpEventHub>(implementationFactory:serviceProvider =>
            new HttpEventHub(serviceProvider.GetRequiredService<IHttpEventProcessingService>()));

        services.AddHostedService<HttpEventDispatcherHostedService>();
    }
}