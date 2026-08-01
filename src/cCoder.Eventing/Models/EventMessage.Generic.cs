// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Models;

public class EventMessage<T> : EventMessage
{
    public T Data { get; set; }
}