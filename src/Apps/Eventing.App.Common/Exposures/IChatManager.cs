// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Exposures;

public interface IChatManager
{
    ValueTask<ChatMessage> SendChatMessageAsync(
        ChatMessage newChatMessage,
        CancellationToken cancellationToken = default);

    ValueTask ReceiveChatMessageAsync(ChatMessage chatMessage);
}
