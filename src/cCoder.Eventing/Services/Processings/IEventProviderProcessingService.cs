// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Processings;

internal interface IEventProviderProcessingService
{
    ValueTask<bool> RaiseEventAsync<T>(string name, EventMessage<T> message);
}