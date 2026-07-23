// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal interface IEventAuthorizationBroker
{
    IEventAuthInfo GetEventAuthInfo();
}