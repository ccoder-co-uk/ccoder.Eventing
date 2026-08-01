// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public sealed class HttpEventingConfiguration
{
    public HttpEventingConfiguration()
    {
        HubUrl = string.Empty;
        MaxConcurrency = 1;
    }

    public string HubUrl { get; set; }
    public int MaxConcurrency { get; set; }
}