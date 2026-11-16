using System.Collections.Concurrent;
using cCoder.Data.Models.Eventing;
using cCoder.Eventing;
using cCoder.Eventing.Models;
using cCoder.Eventing.Sql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Xunit;

namespace cCoder.Eventing.Sql.Tests;

public sealed class SqlEventWorkerTests
{
    [Fact]
    public async Task WorkerUsesItsScopeAndEventHubCreatesHandlerScope()
    {
        FakeStore store = new();
        IServiceProvider? workerProvider = null;
        IServiceProvider? handlerProvider = null;
        string? handled = null;
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
        {
            workerProvider = provider;
            return provider.GetRequiredService<IEventHub>()
                .RaiseEventAsync("sample", message);
        });

        services.GetRequiredService<IEventHub>()
            .ListenToEvent<string, IServiceProvider>("sample", (provider, data) =>
            {
                handlerProvider = provider;
                handled = data;
                return ValueTask.CompletedTask;
            });

        await services.GetRequiredService<ISqlEventHub>().RaiseEventAsync(
            "sample", NewMessage());
        await RunWorkerUntilAsync(services, store.Completed.Task);

        Assert.Equal("payload", handled);
        Assert.NotNull(workerProvider);
        Assert.NotNull(handlerProvider);
        Assert.NotSame(workerProvider, handlerProvider);
        Assert.Empty(store.Failed);
    }

    [Fact]
    public async Task UnhandledExceptionMovesMessageToFailuresWithoutAutomaticRetry()
    {
        FakeStore store = new();
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));

        services.GetRequiredService<IEventHub>()
            .ListenToEvent<string, IServiceProvider>("sample", (_, _) =>
                throw new InvalidOperationException("handler failed"));

        await services.GetRequiredService<ISqlEventHub>().RaiseEventAsync(
            "sample", NewMessage());
        await RunWorkerUntilAsync(services, store.FailureRecorded.Task);

        FailedEventMessageRecord failed = Assert.Single(store.Failed);
        Assert.Contains("handler failed", failed.FailureReason);
        Assert.Equal(0, store.RetryCount);
    }

    [Fact]
    public async Task HandlerCanRequestDelayedRetry()
    {
        FakeStore store = new();
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));

        services.GetRequiredService<IEventHub>()
            .ListenToEvent<string, IServiceProvider>("sample", (provider, _) =>
            {
                provider.GetRequiredService<ISqlEventRetryContext>()
                    .RequestRetry(TimeSpan.FromSeconds(5));
                return ValueTask.CompletedTask;
            });

        await services.GetRequiredService<ISqlEventHub>().RaiseEventAsync(
            "sample", NewMessage());
        await RunWorkerUntilAsync(services, store.RetryRequested.Task);

        Assert.Equal(1, store.RetryCount);
        Assert.Empty(store.Failed);
    }

    [Fact]
    public async Task MissingReceiverRecordsFailedEvent()
    {
        FakeStore store = new();
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));

        await services.GetRequiredService<ISqlEventHub>().RaiseEventAsync(
            "missing", NewMessage());
        await RunWorkerUntilAsync(services, store.FailureRecorded.Task);

        Assert.Contains("No handler", Assert.Single(store.Failed).FailureReason);
    }

    [Fact]
    public async Task FailureAndQueueHubsExposeOperatorActions()
    {
        FakeStore store = new();
        Guid failedId = Guid.NewGuid();
        store.Failed.Add(new FailedEventMessageRecord
        {
            Id = failedId,
            CreatedOn = DateTimeOffset.UtcNow,
            EventName = "sample",
            Data = "{}",
            FailureReason = "bad payload"
        });
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));

        ISqlEventFailureHub failures = services.GetRequiredService<ISqlEventFailureHub>();
        Assert.Equal(failedId, Assert.Single(await failures.GetFailedAsync(10)).Id);
        Assert.NotNull(await failures.RequeueAsync(failedId));
        Assert.Empty(await failures.GetFailedAsync(10));

        store.Failed.Add(new FailedEventMessageRecord { Id = failedId });
        Assert.True(await failures.RemoveAsync(failedId));
        Assert.False(await failures.RemoveAsync(failedId));
        Assert.Null(await failures.RequeueAsync(failedId));

        SqlEventQueueReport report = await services
            .GetRequiredService<ISqlEventQueueMonitor>()
            .GetQueueReportAsync();
        Assert.Empty(report.Queues);
    }

    [Fact]
    public async Task BulkHubQueuesEachMessageWithItsCaller()
    {
        FakeStore store = new();
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));

        await services.GetRequiredService<ISqlEventHub>().RaiseEventsAsync(
            "sample", [NewMessage(), NewMessage()]);

        Assert.Equal(2, store.QueuedCount);
    }

    [Fact]
    public async Task InvalidEventNamesAndMissingPayloadAreRejected()
    {
        FakeStore store = new();
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));
        ISqlEventHub hub = services.GetRequiredService<ISqlEventHub>();

        await Assert.ThrowsAnyAsync<Exception>(() =>
            hub.RaiseEventAsync("", NewMessage()).AsTask());
        await Assert.ThrowsAnyAsync<Exception>(() =>
            hub.RaiseEventAsync<string>("sample", null!).AsTask());
        await Assert.ThrowsAnyAsync<Exception>(() =>
            hub.RaiseEventAsync("sample", new EventMessage<string>
            {
                AuthInfo = new EventAuthInfo { SSOUserId = "test-user" },
                Data = null!
            }).AsTask());
        await Assert.ThrowsAnyAsync<Exception>(() =>
            hub.RaiseEventsAsync<string>("sample", null!));
        Assert.Equal(0, store.QueuedCount);
    }

    [Fact]
    public async Task StorageFailureDuringFailureRecordingDoesNotCrashWorker()
    {
        FakeStore store = new() { ThrowOnFail = true };
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));

        await services.GetRequiredService<ISqlEventHub>().RaiseEventAsync(
            "missing", NewMessage());
        await RunWorkerUntilAsync(services, store.FailureRecorded.Task);

        Assert.Empty(store.Failed);
    }

    [Fact]
    public async Task QueueReportStorageErrorIsWrapped()
    {
        FakeStore store = new() { ThrowOnReport = true };
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));

        await Assert.ThrowsAnyAsync<Exception>(() => services
            .GetRequiredService<ISqlEventQueueMonitor>()
            .GetQueueReportAsync());
    }

    [Fact]
    public async Task WorkerPollsForMessagesArrivingAfterStartup()
    {
        FakeStore store = new();
        await using ServiceProvider services = CreateServices(store, (provider, _, message) =>
            provider.GetRequiredService<IEventHub>().RaiseEventAsync("sample", message));
        services.GetRequiredService<IEventHub>()
            .ListenToEvent<string, IServiceProvider>("sample", (_, _) =>
                ValueTask.CompletedTask);

        IHostedService worker = Assert.Single(services.GetServices<IHostedService>());
        await worker.StartAsync(CancellationToken.None);

        try
        {
            await Task.Delay(50);
            await services.GetRequiredService<ISqlEventHub>().RaiseEventAsync(
                "sample", NewMessage());
            await store.Completed.Task.WaitAsync(TimeSpan.FromSeconds(5));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }
    }

    [Fact]
    public async Task InvalidWorkerConcurrencyFailsBeforeClaimingMessages()
    {
        FakeStore store = new();
        await using ServiceProvider services = CreateServices(
            store,
            (provider, _, message) => provider.GetRequiredService<IEventHub>()
                .RaiseEventAsync("sample", message),
            options => options.MaxConcurrency = 0);

        BackgroundService worker = Assert.IsAssignableFrom<BackgroundService>(
            Assert.Single(services.GetServices<IHostedService>()));

        await Assert.ThrowsAnyAsync<Exception>(async () =>
        {
            await worker.StartAsync(CancellationToken.None);
            await worker.ExecuteTask!;
        });

        Assert.Equal(0, store.QueuedCount);
    }

    private static EventMessage<string> NewMessage() => new()
    {
        AuthInfo = new EventAuthInfo { SSOUserId = "test-user" },
        Data = "payload"
    };

    private static ServiceProvider CreateServices(
        FakeStore store,
        Func<IServiceProvider, string, EventMessage<string>, ValueTask> receive,
        Action<cCoder.Eventing.Sql.Models.SqlEventingOptions>? configure = null)
    {
        ServiceCollection registrations = new();
        registrations.AddLogging();
        registrations.AddSingleton<ISqlEventStorage>(store);
        registrations.AddEventing(new EventingConfiguration
        {
            EventProviders =
            [
                new EventProvider<string>
                {
                    Events = ["sample"],
                    ReceiveHandler = receive
                }
            ]
        });
        registrations.AddEventingForType<string>();
        registrations.AddSqlEventingHostedServices(options =>
        {
            options.PollInterval = TimeSpan.FromMilliseconds(20);
            options.LeaseDuration = TimeSpan.FromMinutes(1);
            options.MaxConcurrency = 2;
            configure?.Invoke(options);
        });
        return registrations.BuildServiceProvider();
    }

    private static async Task RunWorkerUntilAsync(
        IServiceProvider services,
        Task outcome)
    {
        IHostedService worker = Assert.Single(services.GetServices<IHostedService>());
        await worker.StartAsync(CancellationToken.None);

        try
        {
            await outcome.WaitAsync(TimeSpan.FromSeconds(5));
        }
        finally
        {
            await worker.StopAsync(CancellationToken.None);
        }
    }

    private sealed class FakeStore : ISqlEventStorage
    {
        private readonly ConcurrentQueue<EventMessageRecord> queued = new();
        public List<FailedEventMessageRecord> Failed { get; } = [];
        public bool ThrowOnFail { get; set; }
        public bool ThrowOnReport { get; set; }
        public int QueuedCount => queued.Count;
        public int RetryCount { get; private set; }
        public TaskCompletionSource Completed { get; } = new(
            TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource FailureRecorded { get; } = new(
            TaskCreationOptions.RunContinuationsAsynchronously);
        public TaskCompletionSource RetryRequested { get; } = new(
            TaskCreationOptions.RunContinuationsAsynchronously);

        public ValueTask EnqueueAsync(EventMessageRecord record, CancellationToken _) 
        {
            queued.Enqueue(record);
            return ValueTask.CompletedTask;
        }

        public Task<IReadOnlyList<EventMessageRecord>> ClaimAsync(
            int count, TimeSpan leaseDuration, CancellationToken _)
        {
            List<EventMessageRecord> batch = [];
            while (batch.Count < count && queued.TryDequeue(out EventMessageRecord? record))
            {
                record.LeaseUntil = DateTimeOffset.UtcNow.Add(leaseDuration);
                batch.Add(record);
            }
            return Task.FromResult<IReadOnlyList<EventMessageRecord>>(batch);
        }

        public Task<bool> CompleteAsync(EventMessageRecord _, CancellationToken cancellationToken)
        {
            Completed.TrySetResult();
            return Task.FromResult(true);
        }

        public Task<bool> FailAsync(EventMessageRecord record, string reason, CancellationToken _)
        {
            if (ThrowOnFail)
            {
                FailureRecorded.TrySetResult();
                throw new InvalidOperationException("failure storage unavailable");
            }

            Failed.Add(new FailedEventMessageRecord
            {
                Id = record.Id,
                CreatedOn = record.CreatedOn,
                EventName = record.EventName,
                Data = record.Data,
                AvailableOn = record.AvailableOn,
                LeaseUntil = record.LeaseUntil,
                Attempt = record.Attempt,
                FailureReason = reason
            });
            FailureRecorded.TrySetResult();
            return Task.FromResult(true);
        }

        public Task<Guid?> RetryAsync(EventMessageRecord _, TimeSpan delay, CancellationToken cancellationToken)
        {
            RetryCount++;
            RetryRequested.TrySetResult();
            return Task.FromResult<Guid?>(Guid.NewGuid());
        }

        public Task<IReadOnlyList<FailedEventMessageRecord>> GetFailedAsync(int count, CancellationToken _) =>
            Task.FromResult<IReadOnlyList<FailedEventMessageRecord>>(Failed.Take(count).ToArray());

        public Task<SqlEventQueueReport> GetQueueReportAsync(CancellationToken _) =>
            ThrowOnReport
                ? throw new InvalidOperationException("report storage unavailable")
                : Task.FromResult(new SqlEventQueueReport { Queues = [] });

        public Task<Guid?> RequeueFailedAsync(Guid failedId, CancellationToken _)
        {
            FailedEventMessageRecord? record = Failed.SingleOrDefault(message => message.Id == failedId);
            if (record is null)
                return Task.FromResult<Guid?>(null);

            Failed.Remove(record);
            return Task.FromResult<Guid?>(Guid.NewGuid());
        }

        public Task<bool> RemoveFailedAsync(Guid failedId, CancellationToken _) =>
            Task.FromResult(Failed.RemoveAll(message => message.Id == failedId) == 1);
    }
}
