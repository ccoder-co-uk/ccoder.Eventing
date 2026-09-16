// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal sealed class BulkEventProviderBroker(
    BulkEventProviderBrokerConfiguration configuration)
    : IBulkEventProviderBroker
{
    public bool CanHandle<T>(string name) =>
        configuration.CanHandle(
            arg1: name,
            arg2: typeof(T));

    public ValueTask HandleAsync<T>(
        IServiceProvider serviceProvider,
        EventMessage<T>[] messages) =>
        configuration.HandleAsync(
            arg1: serviceProvider,
            arg2: messages);
}