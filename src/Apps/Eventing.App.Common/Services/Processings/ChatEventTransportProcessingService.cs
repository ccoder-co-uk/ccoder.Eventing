// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services.Foundations;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Services.Processings;

internal sealed partial class ChatEventTransportProcessingService(
    IEnumerable<IChatEventTransportService> chatEventTransportServices)
    : IChatEventTransportProcessingService
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

            foreach (IChatEventTransportService chatEventTransportService
                in chatEventTransportServices)
            {
                await chatEventTransportService.RaiseChatMessageAsync(
                    eventMessage: eventMessage,
                    cancellationToken: cancellationToken);
            }
        });
}