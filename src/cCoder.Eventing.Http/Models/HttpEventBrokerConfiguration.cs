// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;

namespace cCoder.Eventing.Http.Models;

internal sealed class HttpEventBrokerConfiguration
{
    public string HubUrl { get; set; }

    public JsonSerializerOptions JsonSerializerOptions { get; set; }

    public int MaxConcurrency { get; set; }

    public IReadOnlyCollection<HttpEventProviderBrokerConfiguration>
        EventProviderConfigurations { get; set; }
}