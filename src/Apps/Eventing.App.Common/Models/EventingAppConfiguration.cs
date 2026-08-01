// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Apps.Models;

public sealed class EventingAppConfiguration
{
    public EventingAppConfiguration()
    {
        AppName = "Eventing.App";
        RemoteHubUrl = string.Empty;
    }

    public string AppName { get; set; }

    public string RemoteHubUrl { get; set; }
}