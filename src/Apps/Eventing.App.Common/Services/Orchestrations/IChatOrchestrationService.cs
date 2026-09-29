// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Services.Orchestrations;

public interface IChatOrchestrationService
{
    ValueTask<ChatMessage> SendChatMessageAsync(
        ChatMessage chatMessage,
        CancellationToken cancellationToken = default);

    ValueTask ReceiveChatMessageAsync(ChatMessage chatMessage);
}