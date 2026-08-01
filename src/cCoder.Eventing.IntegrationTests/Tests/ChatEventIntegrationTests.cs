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

public partial class ChatEventIntegrationTests
{
    private readonly List<WebApplication> applications = [];
    private readonly List<HubConnection> hubConnections = [];
    private readonly HttpClient httpClient = new();

    private async Task DisposeAsync()
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
                ApplicationName = typeof(cCoder.Eventing.Apps.IServiceCollectionExtensions)
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

        builder.Services.AddEventingAppCommon(
            applicationConfiguration: builder.Configuration);

        WebApplication app = builder.Build();
        app.StartEventingChat();
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
                {
                    completionSource.TrySetResult(result:message);
                }
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
            {
                return candidate;
            }

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

    [Fact]
    public async Task ShouldSendChatEventFromApp1ToApp2()
    {
        // Given

        string app1Url = GetFreeLocalUrl();
        string app2Url = GetFreeLocalUrl();
        string messageText = $"App1 integration message {Guid.NewGuid()}";
        TaskCompletionSource<ChatMessage> app1ReceivedMessage = new();
        TaskCompletionSource<ChatMessage> app2ReceivedMessage = new();

        await StartChatApplicationAsync(
appDirectory: "Eventing.App1",
appName: "Eventing.App1",
appUrl: app1Url,
remoteHubUrl: $"{app2Url}/Api/Eventing/Http");

        await StartChatApplicationAsync(
appDirectory: "Eventing.App2",
appName: "Eventing.App2",
appUrl: app2Url,
remoteHubUrl: $"{app1Url}/Api/Eventing/Http");

        await ConnectToChatHubAsync(appUrl:app1Url, completionSource:app1ReceivedMessage, expectedText:messageText);
        await ConnectToChatHubAsync(appUrl:app2Url, completionSource:app2ReceivedMessage, expectedText:messageText);

        await SendChatMessageAsync(appUrl:app1Url, user:"Integration", text:messageText);

        ChatMessage app1Message =
            await WaitForMessageAsync(completionSource:app1ReceivedMessage);

        // When

        ChatMessage app2Message =
            await WaitForMessageAsync(completionSource:app2ReceivedMessage);

        // Then

        app1Message.Text.Should()
            .Be(expected:messageText);

        app2Message.Text.Should()
            .Be(expected:messageText);

        app1Message.SourceApp.Should()
            .Be(expected:"Eventing.App1");

        app2Message.SourceApp.Should()
            .Be(expected:"Eventing.App1");

        await DisposeAsync();
    }

    [Fact]
    public async Task ShouldSendChatEventFromApp2ToApp1()
    {
        // Given

        string app1Url = GetFreeLocalUrl();
        string app2Url = GetFreeLocalUrl();
        string messageText = $"App2 integration message {Guid.NewGuid()}";
        TaskCompletionSource<ChatMessage> app1ReceivedMessage = new();
        TaskCompletionSource<ChatMessage> app2ReceivedMessage = new();

        await StartChatApplicationAsync(
appDirectory: "Eventing.App1",
appName: "Eventing.App1",
appUrl: app1Url,
remoteHubUrl: $"{app2Url}/Api/Eventing/Http");

        await StartChatApplicationAsync(
appDirectory: "Eventing.App2",
appName: "Eventing.App2",
appUrl: app2Url,
remoteHubUrl: $"{app1Url}/Api/Eventing/Http");

        await ConnectToChatHubAsync(appUrl:app1Url, completionSource:app1ReceivedMessage, expectedText:messageText);
        await ConnectToChatHubAsync(appUrl:app2Url, completionSource:app2ReceivedMessage, expectedText:messageText);

        await SendChatMessageAsync(appUrl:app2Url, user:"Integration", text:messageText);

        ChatMessage app1Message =
            await WaitForMessageAsync(completionSource:app1ReceivedMessage);

        // When

        ChatMessage app2Message =
            await WaitForMessageAsync(completionSource:app2ReceivedMessage);

        // Then

        app1Message.Text.Should()
            .Be(expected:messageText);

        app2Message.Text.Should()
            .Be(expected:messageText);

        app1Message.SourceApp.Should()
            .Be(expected:"Eventing.App2");

        app2Message.SourceApp.Should()
            .Be(expected:"Eventing.App2");

        await DisposeAsync();
    }
}