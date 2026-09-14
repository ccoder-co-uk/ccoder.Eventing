// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services.Processings;

namespace cCoder.Eventing.Apps.Services.Orchestrations;

internal sealed partial class ChatOrchestrationService(
        IChatEventTransportProcessingService chatEventService,
        IChatNotificationProcessingService chatNotificationService)
            : IChatOrchestrationService
{
    public ValueTask<ChatMessage> SendChatMessageAsync(
        ChatMessage chatMessage,
        CancellationToken cancellationToken = default) =>
        TryCatch<ChatMessage>(operation: async () =>
        {
            ValidateSendChatMessage(
                chatMessage: chatMessage,
                cancellationToken: cancellationToken);

            chatMessage.Id = Guid.NewGuid();

            chatMessage.User =
                string.IsNullOrWhiteSpace(value: chatMessage.User)
                    ? "Guest"
                    : chatMessage.User.Trim();

            chatMessage.Text = chatMessage.Text!.Trim();
            chatMessage.CreatedOn = DateTimeOffset.UtcNow;

            await chatEventService.RaiseChatMessageAsync(
                chatMessage: chatMessage,
                cancellationToken: cancellationToken);

            return chatMessage;
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