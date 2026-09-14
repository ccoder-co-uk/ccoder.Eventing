// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal sealed class BulkEventProviderBroker(BulkEventProvider eventProvider)
    : IBulkEventProviderBroker
{
    public bool CanHandle<T>(string name) =>
        eventProvider.CanHandle<T>(name: name);

    public ValueTask HandleAsync<T>(
        IServiceProvider serviceProvider,
        EventMessage<T>[] messages) =>
        eventProvider.HandleAsync(
            serviceProvider: serviceProvider,
            messages: messages);
}