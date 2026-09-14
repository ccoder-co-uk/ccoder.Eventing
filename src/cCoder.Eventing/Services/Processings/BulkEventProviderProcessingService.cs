// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;

namespace cCoder.Eventing.Services.Processings;

internal sealed partial class BulkEventProviderProcessingService(
        IBulkEventProviderService eventProviderService)
            : IBulkEventProviderProcessingService
{
    public ValueTask<bool> RaiseEventsAsync<T>(string name, EventMessage<T>[] messages) =>
        TryCatch<bool>(operation: async () =>
        {
            Validate(inputs: [name, messages]);
            return await eventProviderService.RaiseEventsAsync(name: name, messages: messages);
        });
}