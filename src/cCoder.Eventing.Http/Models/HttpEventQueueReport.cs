// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Collections.Generic;

namespace cCoder.Eventing.Http.Models;

public sealed class HttpEventQueueReport
{
    public IReadOnlyCollection<HttpEventQueueSnapshot> Queues { get; set; }
}