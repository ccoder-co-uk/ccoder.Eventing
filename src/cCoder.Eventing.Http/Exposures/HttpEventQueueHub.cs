// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Processings;

namespace cCoder.Eventing.Http;

public sealed class HttpEventQueueHub : IHttpEventQueueMonitor
{
    private readonly IHttpEventQueueProcessingService queueService;

    internal HttpEventQueueHub(IHttpEventQueueProcessingService queueService) =>
        this.queueService = queueService;

    public HttpEventQueueReport GetQueueReport() =>
        queueService.GetQueueReport();
}