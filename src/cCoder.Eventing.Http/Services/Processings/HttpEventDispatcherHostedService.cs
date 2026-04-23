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

        await foreach (HttpEventMessage message in eventQueue.ReadAllAsync(stoppingToken))
        {
            await concurrencyGate.WaitAsync(stoppingToken);

            _ = Task.Run(
                async () =>
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
                stoppingToken);
        }
    }

    private int GetMaxConcurrency() =>
        Math.Max(1, options.MaxConcurrency);
}
