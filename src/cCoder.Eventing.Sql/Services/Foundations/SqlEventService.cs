// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Models;
using cCoder.Eventing.Sql.Brokers;
using cCoder.Eventing.Sql.Models;

namespace cCoder.Eventing.Sql.Services.Foundations;

internal sealed partial class SqlEventService(
    ISqlEventStore store,
    ISqlEventTransportBroker transport,
    ISqlEventLoggingBroker logging,
    ISqlEventRetryBroker retry) : ISqlEventService
{
    public Task ProcessAsync(IServiceProvider scopedProvider, CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [scopedProvider, cancellationToken]);

            await ExecuteProcessAsync(
                scopedProvider: scopedProvider,
                cancellationToken: cancellationToken);
        });

    private async Task ExecuteProcessAsync(
        IServiceProvider scopedProvider,
        CancellationToken cancellationToken)
    {
        if (transport.GetMaxConcurrency() < 1 || transport.GetPollInterval() <= TimeSpan.Zero ||
            transport.GetLeaseDuration() <= transport.GetPollInterval())
        {
            throw new InvalidOperationException("Invalid SQL event worker configuration.");
        }

        await ProcessBatchAsync(
            scopedProvider: scopedProvider,
            cancellationToken: cancellationToken);

        using PeriodicTimer timer = new(transport.GetPollInterval());

        while (await timer.WaitForNextTickAsync(cancellationToken: cancellationToken))
        {
            await ProcessBatchAsync(
                scopedProvider: scopedProvider,
                cancellationToken: cancellationToken);
        }
    }

    private async Task ProcessBatchAsync(
        IServiceProvider scopedProvider,
        CancellationToken cancellationToken)
    {
        IReadOnlyList<EventMessageRecord> messages = await store.ClaimAsync(
            count: transport.GetMaxConcurrency(),
            leaseDuration: transport.GetLeaseDuration(),
            cancellationToken: cancellationToken);

        await Task.WhenAll(tasks: messages.Select(selector: message =>
            ProcessOneAsync(
                scopedProvider: scopedProvider,
                eventMessageRecord: message,
                cancellationToken: cancellationToken)));
    }

    private async Task ProcessOneAsync(
        IServiceProvider scopedProvider,
        EventMessageRecord eventMessageRecord,
        CancellationToken cancellationToken)
    {
        try
        {
            retry.Reset();

            SqlEventEnvelope envelope = transport.DeserializeEnvelope(data: eventMessageRecord.Data)
                ?? throw new InvalidOperationException("SQL event payload is empty.");

            EventProvider[] receivers = transport.GetReceivers(name: eventMessageRecord.EventName);

            if (receivers.Length == 0)
            {
                throw new InvalidOperationException(
                    $"No handler is registered for SQL event '{eventMessageRecord.EventName}'.");
            }

            foreach (EventProvider receiver in receivers)
            {
                cancellationToken.ThrowIfCancellationRequested();

                EventMessage eventMessage = transport.CreateMessage(
                    eventProvider: receiver,
                    sqlEventEnvelope: envelope);

                await transport.ReceiveAsync(
                    scopedProvider: scopedProvider,
                    eventProvider: receiver,
                    name: eventMessageRecord.EventName,
                    eventMessage: eventMessage);
            }

            TimeSpan? delay = retry.GetRequestedDelay();

            if (delay is not null)
            {
                Guid? newId = await store.RetryAsync(
                    eventMessageRecord: eventMessageRecord,
                    delay: delay.Value,
                    cancellationToken: cancellationToken);

                logging.LogWarning(
                    message: "Handler requested retry of event {EventName} {MessageId} as {RequeuedMessageId}",
                    args: [eventMessageRecord.EventName, eventMessageRecord.Id, newId]);
            }
            else
            {
                await store.CompleteAsync(
                    eventMessageRecord: eventMessageRecord,
                    cancellationToken: cancellationToken);
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
        }
        catch (Exception exception)
        {
            try
            {
                bool recorded = await store.FailAsync(
                    eventMessageRecord: eventMessageRecord,
                    reason: exception.GetBaseException().Message,
                    cancellationToken: cancellationToken);

                logging.LogError(
                    exception: exception,
                    message: "SQL event {EventName} {MessageId} failed; recorded={Recorded}",
                    args: [eventMessageRecord.EventName, eventMessageRecord.Id, recorded]);
            }
            catch (Exception storageException)
            {
                logging.LogError(
                    exception: storageException,
                    message: "Could not record failure of SQL event {EventName} {MessageId}",
                    args: [eventMessageRecord.EventName, eventMessageRecord.Id]);
            }
        }
    }

    public Task<Guid> RaiseEventAsync<T>(
        string name,
        EventMessage<T> eventMessage,
        CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, eventMessage, cancellationToken]);
            ValidateMessage(name: name, message: eventMessage);

            DateTimeOffset now = DateTimeOffset.UtcNow;
            Guid id = Guid.NewGuid();
            string data = transport.Serialize(eventMessage: eventMessage);

            await store.EnqueueAsync(
                eventMessageRecord: new EventMessageRecord
                {
                    Id = id,
                    CreatedOn = now,
                    AvailableOn = now,
                    EventName = name,
                    Data = data
                },
                cancellationToken: cancellationToken);

            return id;
        });

    public Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(
        int count,
        CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [count, cancellationToken]);
            ValidateFailedOnGet(count: count);
            return store.GetFailedAsync(count: count, cancellationToken: cancellationToken);
        });

    public Task<Guid?> RequeueAsync(Guid failedId, CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [failedId, cancellationToken]);

            Guid? queuedId = await store.RequeueFailedAsync(
                failedId: failedId,
                cancellationToken: cancellationToken);

            if (queuedId is not null)
            {
                logging.LogInformation(
                    message: "Failed event {FailedMessageId} requeued as {QueuedMessageId}",
                    args: [failedId, queuedId]);
            }

            return queuedId;
        });

    public Task<bool> RemoveAsync(Guid failedId, CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [failedId, cancellationToken]);

            bool removed = await store.RemoveFailedAsync(
                failedId: failedId,
                cancellationToken: cancellationToken);

            if (removed)
            {
                logging.LogInformation(
                    message: "Failed event {FailedMessageId} removed",
                    args: [failedId]);
            }

            return removed;
        });
}