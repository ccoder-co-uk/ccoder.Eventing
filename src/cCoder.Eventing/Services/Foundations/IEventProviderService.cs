// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Foundations;

internal interface IEventProviderService
{
    ValueTask<bool> RaiseEventAsync<T>(string name, EventMessage<T> message);
}