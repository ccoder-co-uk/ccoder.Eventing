// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services.Foundations;

namespace cCoder.Eventing.Apps.Services.Processings;

internal sealed partial class ChatNotificationProcessingService(
    IChatNotificationService chatNotificationService)
    : IChatNotificationProcessingService
{
    public ValueTask SendChatMessageAsync(ChatMessage chatMessage) =>
        TryCatch(operation: async () =>
        {
            ValidateSendChatMessage(chatMessage: chatMessage);

            await chatNotificationService.SendChatMessageAsync(
                chatMessage: chatMessage);
        });
}