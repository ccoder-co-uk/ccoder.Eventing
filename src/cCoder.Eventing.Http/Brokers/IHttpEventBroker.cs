// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Http.Brokers;

internal interface IHttpEventBroker
{
    bool IsConfigured();

    int SelectMaxConcurrency();

    string Serialize(object value);

    ValueTask SendAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default);

    ValueTask EnqueueAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default);

    IAsyncEnumerable<HttpEventMessage> ReadAllAsync(
        CancellationToken cancellationToken = default);

    void InsertSubscription<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler);

    IReadOnlyCollection<HttpEventSubscription> SelectSubscriptions(
        string name);

    object Deserialize(
        HttpEventSubscription httpEventSubscription,
        HttpEventMessage httpEventMessage);

    IReadOnlyCollection<HttpEventProviderBrokerConfiguration>
        SelectEventProviderConfigurations(string name);

    EventMessage CreateMessage(
        HttpEventProviderBrokerConfiguration
            httpEventProviderBrokerConfiguration,
        HttpEventMessage httpEventMessage);

    ValueTask HandleAsync(
        HttpEventProviderBrokerConfiguration
            httpEventProviderBrokerConfiguration,
        IServiceProvider serviceProvider,
        string eventName,
        EventMessage eventMessage);
}