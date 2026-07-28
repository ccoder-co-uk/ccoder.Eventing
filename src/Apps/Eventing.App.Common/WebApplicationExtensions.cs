// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Dependencies;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services.Orchestrations;
using cCoder.Eventing.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

namespace cCoder.Eventing.Apps;

public static class WebApplicationExtensions
{
    public static WebApplication StartEventingChat(this WebApplication app)
    {
        IEventHub eventHub = app.Services.GetRequiredService<IEventHub>();
        IHttpEventHub httpEventHub = app.Services.GetRequiredService<IHttpEventHub>();

        eventHub.ListenToEvent<ChatMessage, IChatOrchestrationService>(
            name: ChatEventNames.ChatEvent,
            handler: static (service, message) =>
                service.ReceiveAsync(message: message));

        httpEventHub.ListenToEvent<ChatMessage>(
            name: ChatEventNames.ChatEvent,
            handler: static (serviceProvider, message) =>
                serviceProvider
                    .GetRequiredService<IChatOrchestrationService>()
                    .ReceiveAsync(message: message));

        string sharedWebRoot =
            GetSharedWebRoot(contentRootPath: app.Environment.ContentRootPath);
        PhysicalFileProvider webRootFileProvider = new(sharedWebRoot);

        app.UseDefaultFiles(options: new DefaultFilesOptions
        {
            FileProvider = webRootFileProvider
        });

        app.UseStaticFiles(options: new StaticFileOptions
        {
            FileProvider = webRootFileProvider
        });

        app.MapControllers();
        app.MapHub<ChatHub>(pattern: "/Api/Hubs/Chat");
        app.MapGet(pattern: "/Health", handler: () => Results.Ok(value: "OK"));
        app.MapGet(
            pattern: "/Api/Chat/Config",
            handler: (EventingAppCommonConfiguration configuration) =>
                Results.Ok(value: configuration));

        return app;
    }

    private static string GetSharedWebRoot(string contentRootPath)
    {
        string sharedWebRoot = Path.GetFullPath(
            path: Path.Combine(
                path1: contentRootPath,
                path2: "..",
                path3: "Eventing.App.Common",
                path4: "wwwroot"));

        if (Directory.Exists(path: sharedWebRoot))
        {
            return sharedWebRoot;
        }

        DirectoryInfo? directory = new(contentRootPath);

        while (directory is not null)
        {
            string candidate = Path.Combine(
                paths:
                [
                    directory.FullName,
                    "src",
                    "Apps",
                    "Eventing.App.Common",
                    "wwwroot"
                ]);

            if (Directory.Exists(path: candidate))
            {
                return candidate;
            }

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Could not find the Eventing app web root.");
    }
}