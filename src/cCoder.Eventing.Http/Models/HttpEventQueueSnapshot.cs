// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

namespace cCoder.Eventing.Http.Models;

public sealed class HttpEventQueueSnapshot
{
    public string Name { get; set; }

    public long Queued { get; set; }

    public long Processing { get; set; }

    public long Completed { get; set; }

    public long Failed { get; set; }
}