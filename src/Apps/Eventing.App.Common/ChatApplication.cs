// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Hubs;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services;
using cCoder.Eventing.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Apps;

public static class ChatApplication
{
    public static WebApplicationBuilder Configure(WebApplicationBuilder builder)
    {
        ChatConfiguration configuration = new();
        builder.Configuration.GetSection("EventingChat").Bind(configuration);

        builder.Services.AddSingleton(configuration);
        builder.Services.AddSingleton<IChatOrchestrationService, ChatOrchestrationService>();
        builder.Services.AddSignalR();
        builder.Services.AddControllers().AddHttpEventingControllers();
        builder.Services.AddEventing();
        builder.Services.AddEventingForType<ChatMessage>();
        builder.Services.AddHttpEventingWeb(options =>
            options.HubUrl = configuration.RemoteHubUrl);

        return builder;
    }

    public static WebApplication Start(WebApplication app)
    {
        IEventHub eventHub = app.Services.GetRequiredService<IEventHub>();
        IHttpEventHub httpEventHub = app.Services.GetRequiredService<IHttpEventHub>();

        eventHub.ListenToEvent<ChatMessage, IChatOrchestrationService>(
            ChatEventNames.ChatEvent,
            static (service, message) => service.ReceiveAsync(message));

        httpEventHub.ListenToEvent<ChatMessage>(
            ChatEventNames.ChatEvent,
            static (serviceProvider, message) =>
                serviceProvider.GetRequiredService<IChatOrchestrationService>().ReceiveAsync(message));

        string sharedWebRoot = GetSharedWebRoot(app.Environment.ContentRootPath);
        PhysicalFileProvider webRootFileProvider = new(sharedWebRoot);

        app.UseDefaultFiles(new DefaultFilesOptions
        {
            FileProvider = webRootFileProvider
        });

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = webRootFileProvider
        });

        app.MapControllers();
        app.MapHub<ChatHub>("/Api/Hubs/Chat");
        app.MapGet("/Health", () => Results.Ok("OK"));
        app.MapGet("/Api/Chat/Config", (ChatConfiguration configuration) => Results.Ok(configuration));

        return app;
    }

    private static string GetSharedWebRoot(string contentRootPath)
    {
        string sharedWebRoot = Path.GetFullPath(
            Path.Combine(
                contentRootPath,
                "..",
                "Eventing.App.Common",
                "wwwroot"));

        if (Directory.Exists(sharedWebRoot))
            return sharedWebRoot;

        DirectoryInfo? directory = new(contentRootPath);

        while (directory is not null)
        {
            string candidate = Path.Combine(
                directory.FullName,
                "src",
                "Apps",
                "Eventing.App.Common",
                "wwwroot");

            if (Directory.Exists(candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find the Eventing app web root.");
    }
}