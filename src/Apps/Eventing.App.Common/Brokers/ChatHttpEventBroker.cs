// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Http;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Brokers;

internal sealed class ChatHttpEventBroker(IHttpEventHub httpEventHub)
    : IChatHttpEventBroker
{
    public ValueTask RaiseChatMessageAsync(
        string name,
        EventMessage<ChatMessage> message,
        CancellationToken cancellationToken = default) =>
        httpEventHub.RaiseEventAsync(
            name: name,
            message: message,
            cancellationToken: cancellationToken);
}