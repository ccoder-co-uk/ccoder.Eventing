// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Eventing.Models;

namespace cCoder.Eventing.Services.Foundations;

internal interface IEventService<T>
{
    void ListenToEvent(string name, Func<IServiceProvider, T, ValueTask> handler);
    ValueTask RaiseEventAsync(string name, EventMessage<T> message);
}