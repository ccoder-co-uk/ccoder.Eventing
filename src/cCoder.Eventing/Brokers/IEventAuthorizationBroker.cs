// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal interface IEventAuthorizationBroker
{
    void SetEventMessage(EventMessage eventMessage);

    IEventAuthInfo GetEventAuthInfo();
}