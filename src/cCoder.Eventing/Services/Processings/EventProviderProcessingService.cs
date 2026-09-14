// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;

namespace cCoder.Eventing.Services.Processings;

internal sealed partial class EventProviderProcessingService(
        IEventProviderService eventProviderService)
            : IEventProviderProcessingService
{
    public ValueTask<bool> RaiseEventAsync<T>(string name, EventMessage<T> message) =>
        TryCatch<bool>(operation: async () =>
        {
            Validate(inputs: [name, message]);
            return await eventProviderService.RaiseEventAsync(name: name, message: message);
        });
}