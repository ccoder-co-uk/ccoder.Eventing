// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

internal sealed class BulkEventProviderBrokerConfiguration
{
    public Func<string, Type, bool> CanHandle { get; set; }

    public Func<IServiceProvider, Array, ValueTask> HandleAsync { get; set; }
}