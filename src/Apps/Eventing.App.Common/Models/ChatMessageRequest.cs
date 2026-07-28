// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models;

public class ChatMessageRequest
{
    public ChatMessageRequest()
    {
        User = string.Empty;
        Text = string.Empty;
    }

    public string User { get; set; }

    public string Text { get; set; }
}