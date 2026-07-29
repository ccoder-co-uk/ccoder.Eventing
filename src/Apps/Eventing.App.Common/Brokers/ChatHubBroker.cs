// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Dependencies;
using cCoder.Eventing.Apps.Models;
using Microsoft.AspNetCore.SignalR;

namespace cCoder.Eventing.Apps.Brokers;

internal sealed class ChatHubBroker(
    IHubContext<ChatHub> chatHub)
    : IChatHubBroker
{
    public ValueTask SendChatMessageAsync(
        ChatMessage chatMessage) =>
        new(
            chatHub.Clients.All.SendAsync(
                method: "chatReceived",
                arg1: chatMessage));
}