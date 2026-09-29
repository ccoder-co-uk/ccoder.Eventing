// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Exposures;

public interface IChatManager
{
    ValueTask<ChatMessage> SendChatMessageAsync(
        ChatMessage chatMessage,
        CancellationToken cancellationToken = default);

    ValueTask ReceiveChatMessageAsync(ChatMessage chatMessage);
}