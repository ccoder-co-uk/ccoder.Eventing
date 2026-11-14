// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests;

public sealed partial class HttpEventQueueTests
{
    [Fact]
    public async Task ShouldWaitForCapacityAndReportQueueTransitions()
    {
        // Given

        HttpEventBroker broker = new(
            httpClientFactory: Mock.Of<System.Net.Http.IHttpClientFactory>(),
            configuration: new HttpEventBrokerConfiguration
            {
                QueueCapacity = 1,
                EventProviderConfigurations = []
            });

        HttpEventMessage first = new() { EventName = "invoice", Data = "{}" };
        HttpEventMessage second = new() { EventName = "invoice", Data = "{}" };

        broker.RecordQueued(name: first.EventName);
        await broker.EnqueueAsync(httpEventMessage: first);

        broker.RecordQueued(name: second.EventName);

        // When

        ValueTask waitingWrite = broker.EnqueueAsync(httpEventMessage: second);

        Assert.False(condition: waitingWrite.IsCompleted);

        using CancellationTokenSource stop = new();

        await using IAsyncEnumerator<HttpEventMessage> reader = broker
            .ReadAllAsync(cancellationToken: stop.Token)
            .GetAsyncEnumerator();

        Assert.True(condition: await reader.MoveNextAsync());
        broker.RecordStarted(name: reader.Current.EventName);

        await waitingWrite
            .AsTask()
            .WaitAsync(
                timeout: TimeSpan.FromSeconds(value: 5));

        HttpEventQueueSnapshot running = broker
            .GetQueueSnapshots()
            .Single();

        broker.RecordFinished(name: first.EventName, succeeded: true);
        Assert.True(condition: await reader.MoveNextAsync());
        broker.RecordStarted(name: reader.Current.EventName);
        broker.RecordFinished(name: second.EventName, succeeded: false);

        HttpEventQueueSnapshot finished = broker
            .GetQueueSnapshots()
            .Single();

        // Then

        Assert.Equal(expected: 1, actual: running.Queued);
        Assert.Equal(expected: 1, actual: running.Processing);
        Assert.Equal(expected: 0, actual: finished.Queued);
        Assert.Equal(expected: 0, actual: finished.Processing);
        Assert.Equal(expected: 1, actual: finished.Completed);
        Assert.Equal(expected: 1, actual: finished.Failed);
    }
}