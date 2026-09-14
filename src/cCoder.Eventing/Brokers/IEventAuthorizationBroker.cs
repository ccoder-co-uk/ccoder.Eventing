// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Eventing.Brokers;

internal interface IEventAuthorizationBroker : IUtilityBroker
{
    void SetEventMessage(EventMessage eventMessage);

    IEventAuthInfo GetEventAuthInfo();
}