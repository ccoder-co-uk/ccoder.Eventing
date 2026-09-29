// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Services.Processings;

internal interface IChatNotificationProcessingService
{
    ValueTask SendChatMessageAsync(ChatMessage chatMessage);
}