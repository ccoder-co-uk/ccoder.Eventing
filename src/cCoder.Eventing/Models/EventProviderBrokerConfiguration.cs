// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

internal sealed class EventProviderBrokerConfiguration
{
    public Func<string, Type, bool> CanSend { get; set; }

    public Func<IServiceProvider, string, EventMessage, ValueTask> HandleSendAsync { get; set; }
}