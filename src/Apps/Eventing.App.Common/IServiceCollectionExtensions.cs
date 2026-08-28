// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Dependencies;
using cCoder.Eventing.Apps.Brokers;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Exposures;
using cCoder.Eventing.Apps.Services.Foundations;
using cCoder.Eventing.Apps.Services.Orchestrations;
using cCoder.Eventing.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Apps;

public static class IServiceCollectionExtensions
{
    public static void AddAppCommon(
        this IServiceCollection services,
        IConfiguration applicationConfiguration,
        Action<AppConfiguration>? configure = null)
    {
        AppConfiguration configuration = new();
        applicationConfiguration.Bind(instance: configuration);

        configure?.Invoke(obj: configuration);

        services.AddConfiguration(configuration: configuration);
        services.AddBrokers();
        services.AddFoundations();
        services.AddProcessings();
        services.AddOrchestrations();
        services.AddExposures();
    }

    private static IServiceCollection AddConfiguration(
        this IServiceCollection services,
        AppConfiguration configuration)
    {
        services.AddSingleton(implementationInstance: configuration);
        services.AddEventing();
        services.AddEventingForType<ChatMessage>();
        services.AddHttpEventingWeb(
            configure: options =>
                options.HubUrl = configuration.EventingChat.RemoteHubUrl);

        return services;
    }

    private static IServiceCollection AddBrokers(
        this IServiceCollection services)
    {
        services.AddSingleton<IChatHubBroker, ChatHubBroker>();

        return services;
    }

    private static IServiceCollection AddFoundations(
        this IServiceCollection services)
    {
        services.AddSingleton<IChatEventService, ChatEventService>();
        services.AddSingleton<
            IChatNotificationService,
            ChatNotificationService>();

        return services;
    }

    private static IServiceCollection AddProcessings(
        this IServiceCollection services) =>
        services;

    private static IServiceCollection AddOrchestrations(
        this IServiceCollection services)
    {
        services.AddSingleton<IChatOrchestrationService, ChatOrchestrationService>();
        services.AddSingleton<IChatManager, ChatOrchestrationService>();

        return services;
    }

    private static IServiceCollection AddExposures(this IServiceCollection services)
    {
        services.AddSignalR();

        IMvcBuilder mvcBuilder = services.AddControllers();
        mvcBuilder.AddHttpEventingControllers();

        return services;
    }
}