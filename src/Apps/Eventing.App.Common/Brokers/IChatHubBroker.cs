// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Brokers;

internal interface IChatHubBroker
{
    ValueTask SendChatMessageAsync(ChatMessage chatMessage);
}