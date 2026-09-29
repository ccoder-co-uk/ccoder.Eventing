// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Eventing.Apps.Models;

namespace cCoder.Eventing.Apps.Services.Foundations;

internal interface IChatNotificationService
{
    ValueTask SendChatMessageAsync(ChatMessage chatMessage);
}