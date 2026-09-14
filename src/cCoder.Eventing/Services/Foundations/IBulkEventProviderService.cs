// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Foundations;

internal interface IBulkEventProviderService
{
    ValueTask<bool> RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}