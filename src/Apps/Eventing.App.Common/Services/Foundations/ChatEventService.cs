// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Brokers;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal sealed partial class ChatEventService(
    IChatEventBroker chatEventBroker,
    IChatHttpEventBroker chatHttpEventBroker)
    : IChatEventService
{
    public ValueTask RaiseChatMessageAsync(
        ChatMessage chatMessage,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
        {
            ValidateRaiseChatMessage(
                chatMessage: chatMessage,
                cancellationToken: cancellationToken);

            EventMessage<ChatMessage> eventMessage = new()
            {
                AuthInfo = new EventAuthInfo
                {
                    SSOUserId = chatMessage.User
                },
                Data = chatMessage
            };

            await chatEventBroker.RaiseChatMessageAsync(
                name: ChatEventNames.ChatEvent,
                message: eventMessage);

            await chatHttpEventBroker.RaiseChatMessageAsync(
                name: ChatEventNames.ChatEvent,
                message: eventMessage,
                cancellationToken: cancellationToken);
        });
}