// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal interface IChatEventService
{
    ValueTask RaiseChatMessageAsync(
        ChatMessage chatMessage,
        CancellationToken cancellationToken = default);
}