// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Brokers;

internal interface IBulkEventProviderBroker
{
    bool CanHandle<T>(string name);
    ValueTask HandleAsync<T>(
        IServiceProvider serviceProvider,
        EventMessage<T>[] messages);
}