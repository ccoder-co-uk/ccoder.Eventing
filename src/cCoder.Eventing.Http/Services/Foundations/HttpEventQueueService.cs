// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;

namespace cCoder.Eventing.Http.Services.Foundations;

internal sealed partial class HttpEventQueueService(
    IHttpEventBroker httpEventBroker) : IHttpEventQueueService
{
    public HttpEventQueueReport GetQueueReport() =>
        TryCatch(operation: () =>
        {
            ValidateOperation();

            return new HttpEventQueueReport
            {
                Queues = httpEventBroker.GetQueueSnapshots()
            };
        });
}