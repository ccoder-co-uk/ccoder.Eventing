// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.AcceptanceTests.Models;

internal sealed class EventRecord
{
    public required string PayloadValue { get; init; }
    public required string UserId { get; init; }
}