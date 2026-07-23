// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using FluentAssertions;

namespace cCoder.Eventing.IntegrationTests.Tests;

public partial class ChatEventTests
{
    [Fact]
    public async Task ShouldSendChatEventFromApp1ToApp2()
    {
        string app1Url = GetFreeLocalUrl();
        string app2Url = GetFreeLocalUrl();
        string messageText = $"App1 integration message {Guid.NewGuid()}";
        TaskCompletionSource<ChatMessage> app1ReceivedMessage = new();
        TaskCompletionSource<ChatMessage> app2ReceivedMessage = new();

        await StartChatApplicationAsync(
appDirectory:            "Eventing.App1",
appName:            "Eventing.App1",
appUrl:            app1Url,
remoteHubUrl:            $"{app2Url}/Api/Eventing/Http");

        await StartChatApplicationAsync(
appDirectory:            "Eventing.App2",
appName:            "Eventing.App2",
appUrl:            app2Url,
remoteHubUrl:            $"{app1Url}/Api/Eventing/Http");

        await ConnectToChatHubAsync(appUrl:app1Url, completionSource:app1ReceivedMessage, expectedText:messageText);
        await ConnectToChatHubAsync(appUrl:app2Url, completionSource:app2ReceivedMessage, expectedText:messageText);

        await SendChatMessageAsync(appUrl:app1Url, user:"Integration", text:messageText);

        ChatMessage app1Message =
            await WaitForMessageAsync(completionSource:app1ReceivedMessage);

        ChatMessage app2Message =
            await WaitForMessageAsync(completionSource:app2ReceivedMessage);

        app1Message.Text.Should().Be(expected:messageText);
        app2Message.Text.Should().Be(expected:messageText);
        app1Message.SourceApp.Should().Be(expected:"Eventing.App1");
        app2Message.SourceApp.Should().Be(expected:"Eventing.App1");
    }
}