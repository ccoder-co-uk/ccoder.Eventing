// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace cCoder.Eventing.Http.Services.Processings;

internal class HttpEventDispatcherHostedService(
        IHttpEventQueue eventQueue,
        IHttpEventDispatcher eventDispatcher,
        HttpEventingOptions options,
        ILogger<HttpEventDispatcherHostedService> log)
            : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using SemaphoreSlim concurrencyGate = new(GetMaxConcurrency());

        await foreach (HttpEventMessage message in eventQueue.ReadAllAsync(cancellationToken:stoppingToken))
        {
            await concurrencyGate.WaitAsync(cancellationToken:stoppingToken);

            _ = Task.Run(
function:                async () =>
                {
                    try
                    {
                        await eventDispatcher.DispatchAsync(message, stoppingToken);
                    }
                    catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                    {
                        // The host is shutting down; no further handling is required.
                    }
                    catch (Exception ex)
                    {
                        log.LogError(
                            ex,
                            "Exception thrown whilst processing queued HTTP event {EventName}: {Message}",
                            message.EventName,
                            ex.Message);
                    }
                    finally
                    {
                        concurrencyGate.Release();
                    }
                },
cancellationToken:                stoppingToken);
        }
    }

    private int GetMaxConcurrency() =>
        Math.Max(val1:1, val2:options.MaxConcurrency);
}