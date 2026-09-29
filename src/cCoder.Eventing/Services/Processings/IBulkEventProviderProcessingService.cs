// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Processings;

internal interface IBulkEventProviderProcessingService
{
    ValueTask<bool> RaiseEventsAsync<T>(string name, EventMessage<T>[] messages);
}