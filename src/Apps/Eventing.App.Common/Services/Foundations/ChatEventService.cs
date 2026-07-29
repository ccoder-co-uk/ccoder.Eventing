// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Http;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal sealed partial class ChatEventService(
    IEventHub eventHub,
    IHttpEventHub httpEventHub)
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

            await eventHub.RaiseEventAsync(
                name: ChatEventNames.ChatEvent,
                message: eventMessage);

            await httpEventHub.RaiseEventAsync(
                name: ChatEventNames.ChatEvent,
                message: eventMessage,
                cancellationToken: cancellationToken);
        });
}