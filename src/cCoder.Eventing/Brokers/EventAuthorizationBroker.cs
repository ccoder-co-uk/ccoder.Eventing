// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal class EventAuthorizationBroker : IEventAuthorizationBroker
{
    private EventMessage message;

    public void SetEventMessage(EventMessage message) =>
        this.message = message;

    public IEventAuthInfo GetEventAuthInfo() =>
        message?.AuthInfo;
}