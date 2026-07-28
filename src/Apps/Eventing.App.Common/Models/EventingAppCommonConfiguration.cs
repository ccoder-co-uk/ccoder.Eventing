// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models;

public class EventingAppCommonConfiguration
{
    public EventingAppCommonConfiguration()
    {
        AppName = "Eventing.App";
        RemoteHubUrl = string.Empty;
    }

    public string AppName { get; set; }

    public string RemoteHubUrl { get; set; }
}