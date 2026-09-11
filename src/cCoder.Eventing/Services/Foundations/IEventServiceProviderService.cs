// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Foundations;

internal interface IEventServiceProviderService
{
    void ListenToEvent<T>(string name, Func<IServiceProvider, T, ValueTask> handler);
    void ListenToEvent<TMessage, THandlingService>(
        string name,
        Func<THandlingService, TMessage, ValueTask> handler);
    ValueTask RaiseEventAsync<T>(string name, EventMessage<T> message);
    ValueTask RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}