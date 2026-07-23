// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Brokers;

internal interface IHttpEventBroker
{
    ValueTask SendAsync(HttpEventMessage message, CancellationToken cancellationToken = default);
}