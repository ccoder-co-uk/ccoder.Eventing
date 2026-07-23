// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;

namespace cCoder.Eventing.Http.Models;

public class HttpEventingOptions
{
    public const string HttpClientName = "cCoder.Eventing.Http";

    public string HubUrl { get; set; }

    public int MaxConcurrency { get; set; } = 1;

    public JsonSerializerOptions JsonSerializerOptions { get; set; } =
        new(JsonSerializerDefaults.Web);
}