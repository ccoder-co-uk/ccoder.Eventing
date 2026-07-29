// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services.Foundations;

namespace cCoder.Eventing.Apps.Services.Orchestrations;

internal sealed partial class ChatOrchestrationService(
        IChatEventService chatEventService,
        IChatNotificationService chatNotificationService)
            : IChatOrchestrationService
{
    public ValueTask<ChatMessage> SendChatMessageAsync(
        ChatMessage newChatMessage,
        CancellationToken cancellationToken = default) =>
        TryCatch<ChatMessage>(operation: async () =>
        {
            ValidateSendChatMessage(
                newChatMessage: newChatMessage,
                cancellationToken: cancellationToken);

            newChatMessage.Id = Guid.NewGuid();

            newChatMessage.User =
                string.IsNullOrWhiteSpace(value: newChatMessage.User)
                    ? "Guest"
                    : newChatMessage.User.Trim();

            newChatMessage.Text = newChatMessage.Text.Trim();
            newChatMessage.CreatedOn = DateTimeOffset.UtcNow;

            await chatEventService.RaiseChatMessageAsync(
                chatMessage: newChatMessage,
                cancellationToken: cancellationToken);

            return newChatMessage;
        });

    public ValueTask ReceiveChatMessageAsync(ChatMessage chatMessage) =>
        TryCatch(operation: async () =>
        {
            ValidateReceiveChatMessage(
                chatMessage: chatMessage);

            await chatNotificationService.SendChatMessageAsync(
                chatMessage: chatMessage);
        });
}