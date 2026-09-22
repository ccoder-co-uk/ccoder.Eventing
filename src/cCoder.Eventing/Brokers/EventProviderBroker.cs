// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal sealed class EventProviderBroker(
    EventProviderBrokerConfiguration configuration)
    : IEventProviderBroker
{
    public bool CanSend<T>(string name) =>
        configuration.CanSend(
            arg1: name,
            arg2: typeof(T));

    public ValueTask HandleSendAsync<T>(
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage<T> message) =>
        configuration.HandleSendAsync(
            arg1: serviceProvider,
            arg2: eventName,
            arg3: message);
}