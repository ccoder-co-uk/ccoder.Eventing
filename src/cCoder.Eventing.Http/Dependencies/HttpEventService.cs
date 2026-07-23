// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Models;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace cCoder.Eventing.Http.Dependencies;

internal class HttpEventService(
        IHttpEventBroker httpEventBroker,
        IHttpEventQueue httpEventQueue,
        IHttpEventHandlerRegistry eventHandlerRegistry,
        HttpEventingOptions options,
        ILogger<HttpEventService> log)
            : IHttpEventService
{
    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        eventHandlerRegistry.ListenToEvent(name:name, handler:handler);

    public async ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            ValidateRequest(name:name, message:message);

            HttpEventMessage httpEventMessage = new()
            {
                EventName = name,
                SSOUserId = message.AuthInfo.SSOUserId,
                Data = JsonSerializer.Serialize(
value: message.Data,
options: options.JsonSerializerOptions)
            };

            await httpEventBroker.SendAsync(message:httpEventMessage, cancellationToken:cancellationToken);
        }
        catch (Exception ex)
        {
            log.LogError(
                exception: ex,
                message: "Exception thrown whilst raising HTTP event {EventName}: {Message}",
                args: [name, ex.Message]);

            throw;
        }
    }

    public async ValueTask ReceiveEventAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default)
    {
        ValidateRequest(message:message);
        await httpEventQueue.EnqueueAsync(message:message, cancellationToken:cancellationToken);
    }

    private static void ValidateRequest<T>(
        string name,
        EventMessage<T> message)
    {
        if (string.IsNullOrWhiteSpace(value:name))
        {
            throw new InvalidOperationException("You must provide an event name when raising events.");
        }

        if (message is null)
        {
            throw new InvalidOperationException("You must provide a message when raising events.");
        }

        if (message.Data is null)
        {
            throw new InvalidOperationException("You must provide some message data when raising events.");
        }

        if (message.AuthInfo is null)
        {
            throw new InvalidOperationException("You must provide some message auth information when raising events.");
        }
    }

    private static void ValidateRequest(HttpEventMessage message)
    {
        if (message is null)
        {
            throw new InvalidOperationException("You must provide a message when receiving events.");
        }

        if (string.IsNullOrWhiteSpace(value:message.EventName))
        {
            throw new InvalidOperationException("You must provide an event name when receiving events.");
        }

        if (string.IsNullOrWhiteSpace(value:message.Data))
        {
            throw new InvalidOperationException("You must provide message data when receiving events.");
        }
    }
}