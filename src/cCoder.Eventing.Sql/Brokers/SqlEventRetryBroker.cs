// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using cCoder.Eventing.Sql.Models;

namespace cCoder.Eventing.Sql.Brokers;

internal sealed class SqlEventRetryBroker : ISqlEventRetryContext, ISqlEventRetryBroker
{
    private readonly AsyncLocal<SqlEventRetryState> state = new();

    public void RequestRetry(TimeSpan delay)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(value: delay.Ticks, paramName: nameof(delay));
        (state.Value ??= new SqlEventRetryState()).Delay = delay;
    }

    public TimeSpan? GetRequestedDelay() =>
        state.Value?.Delay;

    public void Reset() =>
        state.Value = new SqlEventRetryState();

}