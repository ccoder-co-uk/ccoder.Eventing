// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Models;
using cCoder.Eventing.Http.Brokers.Loggings;

namespace cCoder.Eventing.Http.Services.Processings;

internal partial class HttpEventProcessingService(
    IHttpEventService httpEventService,
    ILoggingBroker loggingBroker)
    : IHttpEventProcessingService
{
    public ValueTask RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, message, cancellationToken]);

            ValidateEventMessage(
                name: name,
                message: message,
                isConfigured: httpEventService.IsConfigured());

            HttpEventMessage httpEventMessage = CreateHttpEventMessage(
                name: name,
                message: message);

            return httpEventService.SendHttpEventMessageAsync(
                httpEventMessage: httpEventMessage,
                cancellationToken: cancellationToken);
        });

    public ValueTask RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, messages, cancellationToken]);

            return RaiseEventsImplementationAsync(
                name: name,
                messages: messages,
                cancellationToken: cancellationToken);
        });

    public void ListenToEvent<T>(
        string name,
        Func<IServiceProvider, T, ValueTask> handler) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, handler]);

            httpEventService.ListenToEvent(
                name: name,
                handler: handler);
        });

    public ValueTask ReceiveHttpEventMessageAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken = default) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [httpEventMessage, cancellationToken]);

            return httpEventService.EnqueueHttpEventMessageAsync(
                httpEventMessage: httpEventMessage,
                cancellationToken: cancellationToken);
        });

    public Task ProcessHttpEventMessagesAsync(
        CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [cancellationToken]);

            return ProcessHttpEventMessagesImplementationAsync(
                cancellationToken: cancellationToken);
        });

    private async ValueTask RaiseEventsImplementationAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken)
    {
        foreach (EventMessage<T> message in messages)
        {
            ValidateEventMessage(
                name: name,
                message: message,
                isConfigured: httpEventService.IsConfigured());

            HttpEventMessage httpEventMessage = CreateHttpEventMessage(
                name: name,
                message: message);

            await httpEventService.SendHttpEventMessageAsync(
                httpEventMessage: httpEventMessage,
                cancellationToken: cancellationToken);
        }
    }

    private async Task ProcessHttpEventMessagesImplementationAsync(
        CancellationToken cancellationToken)
    {
        using SemaphoreSlim concurrencyGate = new(
            initialCount: httpEventService.GetMaxConcurrency());

        await foreach (HttpEventMessage httpEventMessage in
            httpEventService.ReadAllHttpEventMessagesAsync(
                cancellationToken: cancellationToken))
        {
            await concurrencyGate.WaitAsync(
                cancellationToken: cancellationToken);

            _ = Task.Run(
                function: async () =>
                {
                    try
                    {
                        await DispatchHttpEventMessageAsync(
                            httpEventMessage: httpEventMessage,
                            cancellationToken: cancellationToken);
                    }
                    catch (OperationCanceledException)
                        when (cancellationToken.IsCancellationRequested)
                    {
                    }
                    catch (Exception exception)
                    {
                        loggingBroker.LogError(
                            exception: exception,
                            message: "Exception thrown whilst processing queued HTTP event {EventName}: {Message}",
                            args:
                            [
                                httpEventMessage.EventName,
                                exception.Message
                            ]);
                    }
                    finally
                    {
                        concurrencyGate.Release();
                    }
                },
                cancellationToken: cancellationToken);
        }
    }

    private async ValueTask DispatchHttpEventMessageAsync(
        HttpEventMessage httpEventMessage,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        bool handled = await httpEventService.DispatchHttpEventMessageAsync(
            httpEventMessage: httpEventMessage);

        if (!handled)
        {
            loggingBroker.LogError(
                exception: null,
                message: "HTTP event {EventName} was received but no matching handlers were registered.",
                args: [httpEventMessage.EventName]);
        }
    }

    private HttpEventMessage CreateHttpEventMessage<T>(
        string name,
        EventMessage<T> message) =>
        new()
        {
            EventName = name,
            SSOUserId = message.AuthInfo.SSOUserId,
            Data = httpEventService.Serialize(value: message.Data)
        };
}