// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Services.Processings;

internal interface IChatNotificationProcessingService
{
    ValueTask SendChatMessageAsync(ChatMessage chatMessage);
}