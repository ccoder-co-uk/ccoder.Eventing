// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Brokers;

internal interface IChatEventBroker
{
    ValueTask RaiseChatMessageAsync(
        string name,
        EventMessage<ChatMessage> message);
}