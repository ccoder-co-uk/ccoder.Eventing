// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Http.Models;

internal class HttpEventSubscription
{
    public string EventName { get; init; }

    public Type DataType { get; init; }

    public Func<IServiceProvider, object, ValueTask> Handler { get; init; }
}