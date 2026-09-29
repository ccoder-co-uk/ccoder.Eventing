// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Http.Services.Processings;
using Microsoft.Extensions.Hosting;

namespace cCoder.Eventing.Http;

internal sealed class HttpEventDispatcherHostedService(
    IHttpEventProcessingService eventProcessingService)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        eventProcessingService.ProcessHttpEventMessagesAsync(
            cancellationToken: stoppingToken);
}