// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Http.Models;

internal sealed class HttpEventQueueCounters
{
    public long Queued;

    public long Processing;

    public long Completed;

    public long Failed;
}