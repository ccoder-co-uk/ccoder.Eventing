// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal sealed class EventProviderBroker(EventProvider eventProvider)
    : IEventProviderBroker
{
    public bool CanSend<T>(string name) =>
        eventProvider.CanSend<T>(name: name);

    public ValueTask HandleSendAsync<T>(
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage<T> message) =>
        eventProvider.HandleSendAsync(
            serviceProvider: serviceProvider,
            eventName: eventName,
            message: message);
}