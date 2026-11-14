// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http;

public interface IHttpEventQueueMonitor
{
    HttpEventQueueReport GetQueueReport();
}