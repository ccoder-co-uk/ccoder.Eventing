// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Services.Foundations;

internal interface IHttpEventService
{
    bool IsConfigured();

    string Serialize(object value);

    ValueTask SendHttpEventMessageAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default);

    void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler);

    ValueTask EnqueueHttpEventMessageAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default);

    int GetMaxConcurrency();

    IAsyncEnumerable<HttpEventMessage> ReadAllHttpEventMessagesAsync(
        CancellationToken cancellationToken);

    ValueTask<bool> DispatchHttpEventMessageAsync(
        HttpEventMessage httpEventMessage);
}