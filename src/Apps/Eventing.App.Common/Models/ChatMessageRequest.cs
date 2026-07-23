// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models;

public class ChatMessageRequest
{
    public string User { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;
}