// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Models;
using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Services.Processings;

public interface IHttpEventProcessingService
{
    ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default);

    ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default);

    void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler);

    ValueTask ReceiveHttpEventMessageAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default);

    Task ProcessHttpEventMessagesAsync(
        CancellationToken cancellationToken);

}