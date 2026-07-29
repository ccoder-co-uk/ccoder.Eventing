// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Brokers;

internal interface IChatHubBroker
{
    ValueTask SendChatMessageAsync(ChatMessage chatMessage);
}