// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal interface IChatEventTransportService
{
    ValueTask RaiseChatMessageAsync(
        EventMessage<ChatMessage> eventMessage,
        CancellationToken cancellationToken = default);
}