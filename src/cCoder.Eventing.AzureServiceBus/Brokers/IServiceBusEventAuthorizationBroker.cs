// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.CodeAnalysis.Exposures;

namespace cCoder.Eventing.AzureServiceBus.Brokers;

internal interface IServiceBusEventAuthorizationBroker : IUtilityBroker
{
    void SetEventMessage(ServiceBusEventMessage serviceBusEventMessage);

    IServiceBusEventAuthInfo GetEventAuthInfo();
}