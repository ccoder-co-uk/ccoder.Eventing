// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services.Orchestrations;

namespace cCoder.Eventing.Apps.Exposures;

internal sealed class ChatManager(
    IChatOrchestrationService chatOrchestrationService) : IChatManager
{
    public ValueTask<ChatMessage> SendChatMessageAsync(
        ChatMessage chatMessage,
        CancellationToken cancellationToken = default) =>
        chatOrchestrationService.SendChatMessageAsync(
            chatMessage: chatMessage,
            cancellationToken: cancellationToken);

    public ValueTask ReceiveChatMessageAsync(ChatMessage chatMessage) =>
        chatOrchestrationService.ReceiveChatMessageAsync(
            chatMessage: chatMessage);
}