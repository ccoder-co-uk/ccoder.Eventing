// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models;

public class EventingAppCommonConfiguration
{
    public EventingAppCommonConfiguration()
    {
        Eventing = new EventingAppConfiguration();
    }

    public EventingAppConfiguration Eventing { get; set; }
}