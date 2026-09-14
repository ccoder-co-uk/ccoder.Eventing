// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Brokers;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal sealed partial class ChatHttpEventService(
    IChatHttpEventBroker chatHttpEventBroker)
    : IChatEventTransportService
{
    public ValueTask RaiseChatMessageAsync(
        EventMessage<ChatMessage> eventMessage,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: async () =>
        {
            ValidateRaiseChatMessage(
                eventMessage: eventMessage,
                cancellationToken: cancellationToken);

            await chatHttpEventBroker.RaiseChatMessageAsync(
                name: ChatEventNames.ChatEvent,
                message: eventMessage,
                cancellationToken: cancellationToken);
        });
}