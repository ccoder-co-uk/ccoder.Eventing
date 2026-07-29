// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Brokers;
using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal sealed partial class ChatNotificationService(
    IChatHubBroker chatHubBroker)
    : IChatNotificationService
{
    public ValueTask SendChatMessageAsync(ChatMessage chatMessage) =>
        TryCatch(operation: () =>
        {
            ValidateSendChatMessage(chatMessage: chatMessage);

            return chatHubBroker.SendChatMessageAsync(
                chatMessage: chatMessage);
        });
}