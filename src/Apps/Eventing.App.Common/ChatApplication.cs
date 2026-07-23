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
        builder.Configuration.GetSection("EventingChat").Bind(instance:configuration);

        builder.Services.AddSingleton(implementationInstance:configuration);
        builder.Services.AddSingleton<IChatOrchestrationService, ChatOrchestrationService>();
        builder.Services.AddSignalR();
        builder.Services.AddControllers().AddHttpEventingControllers();
        builder.Services.AddEventing();
        builder.Services.AddEventingForType<ChatMessage>();
        builder.Services.AddHttpEventingWeb(configure:options =>
            options.HubUrl = configuration.RemoteHubUrl);

        return builder;
    }

    public static WebApplication Start(WebApplication app)
    {
        IEventHub eventHub = app.Services.GetRequiredService<IEventHub>();
        IHttpEventHub httpEventHub = app.Services.GetRequiredService<IHttpEventHub>();

        eventHub.ListenToEvent<ChatMessage, IChatOrchestrationService>(
name:            ChatEventNames.ChatEvent,
handler:            static (service, message) => service.ReceiveAsync(message));

        httpEventHub.ListenToEvent<ChatMessage>(
name:            ChatEventNames.ChatEvent,
handler:            static (serviceProvider, message) =>
                serviceProvider.GetRequiredService<IChatOrchestrationService>().ReceiveAsync(message));

        string sharedWebRoot = GetSharedWebRoot(contentRootPath:app.Environment.ContentRootPath);
        PhysicalFileProvider webRootFileProvider = new(sharedWebRoot);

        app.UseDefaultFiles(options:new DefaultFilesOptions
        {
            FileProvider = webRootFileProvider
        });

        app.UseStaticFiles(options:new StaticFileOptions
        {
            FileProvider = webRootFileProvider
        });

        app.MapControllers();
        app.MapHub<ChatHub>(pattern:"/Api/Hubs/Chat");
        app.MapGet(pattern:"/Health", handler:() => Results.Ok("OK"));
        app.MapGet(pattern:"/Api/Chat/Config", handler:(ChatConfiguration configuration) => Results.Ok(configuration));

        return app;
    }

    private static string GetSharedWebRoot(string contentRootPath)
    {
        string sharedWebRoot = Path.GetFullPath(
path:            Path.Combine(
                contentRootPath,
                "..",
                "Eventing.App.Common",
                "wwwroot"));

        if (Directory.Exists(path:sharedWebRoot))
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

            if (Directory.Exists(path:candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find the Eventing app web root.");
    }
}