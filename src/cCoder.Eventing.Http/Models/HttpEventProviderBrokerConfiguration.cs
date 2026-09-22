// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using System.Text.Json;

namespace cCoder.Eventing.Http.Models;

internal sealed class HttpEventProviderBrokerConfiguration
{
    public Func<string, bool> CanReceive { get; set; }

    public Type DataType { get; set; }

    public Func<IServiceProvider, string, EventMessage, ValueTask> ReceiveAsync { get; set; }

    public JsonSerializerOptions JsonSerializerOptions { get; set; }
}