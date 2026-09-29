// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Eventing.AzureServiceBus.Models;

namespace cCoder.Eventing.AzureServiceBus.Services.Foundations;

internal interface IServiceBusService
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, ServiceBusEventMessage<T> eventMessage);
}