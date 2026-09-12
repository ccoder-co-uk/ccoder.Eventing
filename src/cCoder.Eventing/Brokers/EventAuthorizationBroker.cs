// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal class EventAuthorizationBroker : IEventAuthorizationBroker
{
    private EventMessage eventMessage;

    public void SetEventMessage(EventMessage eventMessage) =>
        this.eventMessage = eventMessage;

    public IEventAuthInfo GetEventAuthInfo() =>
        eventMessage?.AuthInfo;
}