using cCoder.Eventing.Apps.Hubs;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Http;
using cCoder.Eventing.Models;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Eventing.Apps.Services;

internal class ChatOrchestrationService(
        IEventHub eventHub,
        IHttpEventHub httpEventHub,
        IHubContext<ChatHub> chatHub,
        ChatConfiguration configuration)
            : IChatOrchestrationService
{
    public async ValueTask<ChatMessage> SendAsync(
        ChatMessageRequest request,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);

        ChatMessage message = new()
        {
            Id = Guid.NewGuid(),
            SourceApp = configuration.AppName,
            User = string.IsNullOrWhiteSpace(request.User) ? "Guest" : request.User.Trim(),
            Text = request.Text.Trim(),
            CreatedOn = DateTimeOffset.UtcNow
        };

        EventMessage<ChatMessage> eventMessage = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = message.User },
            Data = message
        };

        await eventHub.RaiseEventAsync(ChatEventNames.ChatEvent, eventMessage);
        await httpEventHub.RaiseEventAsync(ChatEventNames.ChatEvent, eventMessage, cancellationToken);

        return message;
    }

    public async ValueTask ReceiveAsync(ChatMessage message)
    {
        if (message is null)
            return;

        await chatHub.Clients.All.SendAsync(
            "chatReceived",
            message);
    }

    private static void ValidateRequest(ChatMessageRequest request)
    {
        if (request is null)
            throw new InvalidOperationException("You must provide a chat message.");

        if (string.IsNullOrWhiteSpace(request.Text))
            throw new InvalidOperationException("You must provide chat message text.");
    }
}
