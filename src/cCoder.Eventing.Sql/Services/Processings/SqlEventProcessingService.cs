// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing.Models;
using cCoder.Eventing.Sql.Services.Foundations;

namespace cCoder.Eventing.Sql.Services.Processings;

internal sealed partial class SqlEventProcessingService(ISqlEventService service)
    : ISqlEventProcessingService
{
    public Task ProcessAsync(IServiceProvider scopedProvider, CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [scopedProvider, cancellationToken]);

            return service.ProcessAsync(
                scopedProvider: scopedProvider,
                cancellationToken: cancellationToken);
        });

    public ValueTask<Guid> RaiseEventAsync<T>(
        string name,
        EventMessage<T> eventMessage,
        CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [name, eventMessage, cancellationToken]);

            return new ValueTask<Guid>(service.RaiseEventAsync(
                name: name,
                eventMessage: eventMessage,
                cancellationToken: cancellationToken));
        });

    public Task RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] eventMessages,
        CancellationToken cancellationToken) =>
        TryCatch(operation: async () =>
        {
            Validate(inputs: [name, eventMessages, cancellationToken]);

            foreach (EventMessage<T> eventMessage in eventMessages)
            {
                await service.RaiseEventAsync(
                    name: name,
                    eventMessage: eventMessage,
                    cancellationToken: cancellationToken);
            }
        });

    public Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(
        int count,
        CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [count, cancellationToken]);
            ValidateFailedOnGet(count: count);
            return service.GetFailedAsync(count: count, cancellationToken: cancellationToken);
        });

    public Task<Guid?> RequeueAsync(Guid failedId, CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [failedId, cancellationToken]);
            return service.RequeueAsync(failedId: failedId, cancellationToken: cancellationToken);
        });

    public Task<bool> RemoveAsync(Guid failedId, CancellationToken cancellationToken) =>
        TryCatch(operation: () =>
        {
            Validate(inputs: [failedId, cancellationToken]);
            return service.RemoveAsync(failedId: failedId, cancellationToken: cancellationToken);
        });
}