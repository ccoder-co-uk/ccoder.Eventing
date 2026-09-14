// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Services.Processings;

internal interface IChatEventTransportProcessingService
{
    ValueTask RaiseChatMessageAsync(
        ChatMessage chatMessage,
        CancellationToken cancellationToken = default);
}