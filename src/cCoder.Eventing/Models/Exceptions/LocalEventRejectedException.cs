// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;

namespace cCoder.Eventing.Models.Exceptions;

/// <summary>A local subscriber rejected an operation. The code is interpreted by the publisher.</summary>
public sealed class LocalEventRejectedException(string code)
    : Exception(message: "A local event handler rejected the operation.")
{
    public string Code { get; } = code;
}