using cCoder.Eventing.Apps.Models;
using FluentAssertions;

namespace cCoder.Eventing.IntegrationTests.Tests;

public partial class ChatEventTests
{
    [Fact]
    public async Task ShouldSendChatEventFromApp2ToApp1()
    {
        string app1Url = GetFreeLocalUrl();
        string app2Url = GetFreeLocalUrl();
        string messageText = $"App2 integration message {Guid.NewGuid()}";
        TaskCompletionSource<ChatMessage> app1ReceivedMessage = new();
        TaskCompletionSource<ChatMessage> app2ReceivedMessage = new();

        await StartChatApplicationAsync(
            "Eventing.App1",
            "Eventing.App1",
            app1Url,
            $"{app2Url}/Api/Eventing/Http");

        await StartChatApplicationAsync(
            "Eventing.App2",
            "Eventing.App2",
            app2Url,
            $"{app1Url}/Api/Eventing/Http");

        await ConnectToChatHubAsync(app1Url, app1ReceivedMessage, messageText);
        await ConnectToChatHubAsync(app2Url, app2ReceivedMessage, messageText);

        await SendChatMessageAsync(app2Url, "Integration", messageText);

        ChatMessage app1Message =
            await WaitForMessageAsync(app1ReceivedMessage);

        ChatMessage app2Message =
            await WaitForMessageAsync(app2ReceivedMessage);

        app1Message.Text.Should().Be(messageText);
        app2Message.Text.Should().Be(messageText);
        app1Message.SourceApp.Should().Be("Eventing.App2");
        app2Message.SourceApp.Should().Be("Eventing.App2");
    }
}
