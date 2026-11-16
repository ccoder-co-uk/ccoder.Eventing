// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Text.Json;

namespace cCoder.Eventing.Sql.Models;

public sealed class SqlEventingOptions
{
    public int MaxConcurrency { get; set; }

    public TimeSpan PollInterval { get; set; }

    public TimeSpan LeaseDuration { get; set; }

    public JsonSerializerOptions JsonSerializerOptions { get; set; }
}