// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Processings;

internal interface IBulkEventProviderProcessingService
{
    ValueTask<bool> RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}