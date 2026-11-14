// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;

namespace cCoder.Eventing.Http.Services.Processings;

internal sealed partial class HttpEventQueueProcessingService(
    IHttpEventQueueService queueService) : IHttpEventQueueProcessingService
{
    public HttpEventQueueReport GetQueueReport() =>
        TryCatch(operation: () =>
        {
            ValidateOperation();

            return queueService.GetQueueReport();
        });
}