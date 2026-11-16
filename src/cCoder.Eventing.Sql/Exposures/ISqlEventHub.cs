// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Eventing.Models;

namespace cCoder.Eventing.Sql;

public interface ISqlEventHub
{
    ValueTask<Guid> RaiseEventAsync<T>(
        string name,
        EventMessage<T> message,
        CancellationToken cancellationToken = default);

    Task RaiseEventsAsync<T>(
        string name,
        EventMessage<T>[] messages,
        CancellationToken cancellationToken = default);
}