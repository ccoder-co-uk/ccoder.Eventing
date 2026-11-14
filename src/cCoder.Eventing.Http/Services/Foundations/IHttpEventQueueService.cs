// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Services.Foundations;

internal interface IHttpEventQueueService
{
    HttpEventQueueReport GetQueueReport();
}