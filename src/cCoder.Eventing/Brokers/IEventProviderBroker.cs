// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal interface IEventProviderBroker
{
    bool CanSend<T>(string name);
    ValueTask HandleSendAsync<T>(
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage<T> message);
}