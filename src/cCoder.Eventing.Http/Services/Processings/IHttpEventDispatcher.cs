// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Services.Processings;

internal interface IHttpEventDispatcher
{
    ValueTask DispatchAsync(
        HttpEventMessage message,
        CancellationToken cancellationToken = default);
}