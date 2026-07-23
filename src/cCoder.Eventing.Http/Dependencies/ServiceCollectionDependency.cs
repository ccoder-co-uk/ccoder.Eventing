// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace cCoder.Eventing.Http.Dependencies;

internal static class ServiceCollectionDependency
{
    internal static void AddHttpEventingHostedServices(
        IServiceCollection services,
        Action<HttpEventingOptions> configure)
    {
        AddHttpEventing(
            services: services,
            configure: configure);

        services.AddControllers()
            .AddHttpEventingControllers();
    }

    internal static void AddHttpEventing(
        IServiceCollection services,
        Action<HttpEventingOptions> configure)
    {
        HttpEventingOptions options = new();
        configure?.Invoke(obj: options);

        services.TryAddSingleton(instance: options);
        services.AddHttpClient(name: HttpEventingOptions.HttpClientName);
        services.TryAddSingleton<IHttpEventQueue, HttpEventQueue>();
        services.TryAddSingleton<IHttpEventHandlerRegistry, HttpEventHandlerRegistry>();
        services.TryAddSingleton<IHttpEventBroker, HttpEventBroker>();
        services.TryAddSingleton<IHttpEventDispatcher, HttpEventDispatcher>();
        services.TryAddTransient<IHttpEventService, HttpEventService>();
        services.TryAddTransient<IHttpEventProcessingService, HttpEventProcessingService>();

        services.TryAddSingleton<IHttpEventHub>(implementationFactory: serviceProvider =>
            new HttpEventHub(
                serviceProvider.GetRequiredService<IHttpEventProcessingService>()));

        services.AddHostedService<HttpEventDispatcherHostedService>();
    }
}