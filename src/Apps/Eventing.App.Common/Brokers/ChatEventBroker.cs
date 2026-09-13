// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Brokers;

internal sealed class ChatEventBroker(IEventHub eventHub)
    : IChatEventBroker
{
    public ValueTask RaiseChatMessageAsync(
        string name,
        EventMessage<ChatMessage> message) =>
        eventHub.RaiseEventAsync(
            name: name,
            message: message);
}