// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models;

public sealed class AppConfiguration
{
    public AppConfiguration()
    {
        EventingChat = new EventingChatConfiguration();
    }

    public EventingChatConfiguration EventingChat { get; set; }
}