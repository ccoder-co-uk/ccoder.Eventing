// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Linq;
using System.Text.Json;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Models;
using cCoder.Eventing.Sql.Brokers;
using cCoder.Eventing.Sql.Models;
using cCoder.Eventing.Sql.Services.Processings;
using cCoder.Eventing.Sql.Services.Foundations;
using cCoder.Eventing.Sql.Exposures.HostedServices;
using cCoder.Eventing.Sql.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace cCoder.Eventing.Sql;

public static class IServiceCollectionExtensions
{
    public static void AddSqlEventingWeb(
        this IServiceCollection services,
        Action<SqlEventingOptions> configure = null)
    {
        SqlEventingOptions options = services.CreateOptions(configure: configure);
        services.AddConfiguration(options: options);
        services.AddDependencies();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddExposures();
    }

    public static void AddSqlEventingHostedServices(
        this IServiceCollection services,
        Action<SqlEventingOptions> configure = null)
    {
        SqlEventingOptions options = services.CreateOptions(configure: configure);
        services.AddConfiguration(options: options);
        services.AddDependencies();
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddExposures();
        services.AddHostedService<SqlEventBackgroundService>();
    }

    private static SqlEventingOptions CreateOptions(
        this IServiceCollection services,
        Action<SqlEventingOptions> configure)
    {
        SqlEventingOptions options = new()
        {
            MaxConcurrency = 4,
            PollInterval = TimeSpan.FromMinutes(value: 1),
            LeaseDuration = TimeSpan.FromMinutes(value: 10),
            JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        };
        configure?.Invoke(obj: options);
        return options;
    }

    private static void AddConfiguration(
        this IServiceCollection services,
        SqlEventingOptions options)
    {
        services.TryAddSingleton(instance: options);
        services.TryAddSingleton(implementationFactory: provider =>
            new SqlEventTransportConfiguration
            {
                Options = options,
                Providers = provider.GetServices<EventProvider>().ToArray()
            });
    }

    private static void AddBrokers(this IServiceCollection services)
    {
        services.TryAddScoped<ISqlEventStore, SqlEventBroker>();
        services.TryAddSingleton<ISqlEventTransportBroker, SqlEventTransportBroker>();
        services.TryAddSingleton<ISqlEventLoggingBroker, SqlEventLoggingBroker>();
        services.TryAddSingleton<SqlEventRetryBroker>();
        services.TryAddSingleton<ISqlEventRetryBroker>(implementationFactory: provider =>
            provider.GetRequiredService<SqlEventRetryBroker>());
        services.AddSingleton<ISqlEventRetryContext>(implementationFactory: provider =>
            provider.GetRequiredService<SqlEventRetryBroker>());
    }

    private static void AddDependencies(this IServiceCollection services)
    {
        services.TryAddScoped<ISqlEventStorage, SqlEventStoreDependency>();
        services.TryAddSingleton<SqlEventWorkerScopeDependency>();
    }

    private static void AddFoundations(this IServiceCollection services)
    {
        services.TryAddTransient<ISqlEventService, SqlEventService>();
        services.TryAddTransient<ISqlEventQueueService, SqlEventQueueService>();
    }

    private static void AddProcessings(this IServiceCollection services)
    {
        services.TryAddTransient<ISqlEventProcessingService, SqlEventProcessingService>();
        services.TryAddTransient<ISqlEventQueueProcessingService, SqlEventQueueProcessingService>();
    }

    private static void AddExposures(this IServiceCollection services)
    {
        services.TryAddTransient<ISqlEventHub, SqlEventHub>();
        services.TryAddTransient<ISqlEventFailureHub, SqlEventFailureHub>();
        services.TryAddTransient<ISqlEventQueueMonitor, SqlEventQueueHub>();
    }
}
