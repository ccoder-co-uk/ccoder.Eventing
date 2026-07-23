// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal class EventAuthorizationBroker : IEventAuthorizationBroker
{
    internal EventMessage Message { get; set; }

    public IEventAuthInfo GetEventAuthInfo() =>
        Message?.AuthInfo;
}