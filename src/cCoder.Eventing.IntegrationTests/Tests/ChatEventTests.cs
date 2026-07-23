// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps;
using cCoder.Eventing.Apps.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;

namespace cCoder.Eventing.IntegrationTests.Tests;

public partial class ChatEventTests : IAsyncLifetime
{
    private readonly List<WebApplication> applications = [];
    private readonly List<HubConnection> hubConnections = [];
    private readonly HttpClient httpClient = new();

    public Task InitializeAsync() =>
        Task.CompletedTask;

    public async Task DisposeAsync()
    {
        foreach (HubConnection hubConnection in hubConnections)
        {
            await hubConnection.DisposeAsync();
        }

        foreach (WebApplication application in applications)
        {
            await application.DisposeAsync();
        }

        httpClient.Dispose();
    }

    private async Task<WebApplication> StartChatApplicationAsync(
        string appDirectory,
        string appName,
        string appUrl,
        string remoteHubUrl)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(
options: new WebApplicationOptions
            {
                ContentRootPath = GetAppContentRoot(appDirectory:appDirectory),
                ApplicationName = typeof(ChatApplication)
                    .Assembly.FullName
            });

        builder.WebHost.UseUrls(urls:appUrl);
        builder.Logging.ClearProviders();

        builder.Configuration.AddInMemoryCollection(
initialData: new Dictionary<string, string?>
            {
                ["EventingChat:AppName"] = appName,
                ["EventingChat:RemoteHubUrl"] = remoteHubUrl
            });

        ChatApplication.Configure(builder:builder);

        WebApplication app = builder.Build();
        ChatApplication.Start(app:app);
        await app.StartAsync();

        applications.Add(item:app);

        return app;
    }

    private async Task<HubConnection> ConnectToChatHubAsync(
        string appUrl,
        TaskCompletionSource<ChatMessage> completionSource,
        string expectedText)
    {
        HubConnection hubConnection = new HubConnectionBuilder()
            .WithUrl(url:$"{appUrl}/Api/Hubs/Chat")
            .Build();

        hubConnection.On<ChatMessage>(
methodName: "chatReceived",
handler: message =>
            {
                if (message.Text == expectedText)
                    completionSource.TrySetResult(result:message);
            });

        await hubConnection.StartAsync();
        hubConnections.Add(item:hubConnection);

        return hubConnection;
    }

    private async Task SendChatMessageAsync(
        string appUrl,
        string user,
        string text)
    {
        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(
requestUri: $"{appUrl}/Api/Chat",
value: new ChatMessageRequest
            {
                User = user,
                Text = text
            });

        response.EnsureSuccessStatusCode();
    }

    private static async Task<ChatMessage> WaitForMessageAsync(
        TaskCompletionSource<ChatMessage> completionSource)
    {
        Task completedTask = await Task.WhenAny(
task1: completionSource.Task,
task2: Task.Delay(delay:TimeSpan.FromSeconds(seconds:10)));

        completedTask.Should()
            .BeSameAs(expected:completionSource.Task);

        return await completionSource.Task;
    }

    private static string GetAppContentRoot(string appDirectory)
    {
        DirectoryInfo? directory = new(AppContext.BaseDirectory);

        while (directory is not null)
        {
            string candidate =
                Path.Combine(path1:directory.FullName, path2:"src", path3:"Apps", path4:appDirectory);

            if (Directory.Exists(path:candidate))
                return candidate;

            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            $"Could not find the content root for {appDirectory}.");
    }

    private static string GetFreeLocalUrl()
    {
        using TcpListener listener = new(IPAddress.Loopback, 0);
        listener.Start();

        int port = ((IPEndPoint)listener.LocalEndpoint)
            .Port;

        return $"http://127.0.0.1:{port}";
    }
}