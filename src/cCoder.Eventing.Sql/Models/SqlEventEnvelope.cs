// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Text.Json;

namespace cCoder.Eventing.Sql.Models;

internal sealed class SqlEventEnvelope
{
    public string SSOUserId { get; set; }

    public JsonElement Data { get; set; }
}