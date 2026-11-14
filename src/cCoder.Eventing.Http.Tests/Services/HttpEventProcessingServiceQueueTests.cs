// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;
using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Models.Exceptions;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public sealed partial class HttpEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldRecordRejectedQueueWrite()
    {
        // Given

        HttpEventMessage message = new() { EventName = "invoice", Data = "{}" };
        Mock<IHttpEventService> eventService = new();

        eventService.Setup(expression: value => value.EnqueueHttpEventMessageAsync(
                httpEventMessage: message,
                cancellationToken: It.IsAny<CancellationToken>()))
            .Returns(value: ValueTask.FromException(
                exception: new InvalidOperationException()));

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        Func<Task> enqueue = async () =>
            await service.ReceiveHttpEventMessageAsync(
                httpEventMessage: message);

        // Then

        await Assert.ThrowsAsync<ServiceDependencyException>(testCode: enqueue);

        eventService.Verify(expression: value => value.RecordQueued(
            name: message.EventName), times: Times.Once);

        eventService.Verify(expression: value => value.RecordRejected(
            name: message.EventName), times: Times.Once);
    }

    [Fact]
    public async Task ShouldRecordShutdownWhileWaitingToDispatch()
    {
        // Given

        HttpEventMessage message = new() { EventName = "invoice", Data = "{}" };
        Mock<IHttpEventService> eventService = new();

        eventService.Setup(expression: value => value.GetMaxConcurrency())
            .Returns(value: 1);

        eventService.Setup(expression: value =>
                value.ReadAllHttpEventMessagesAsync(
                    cancellationToken: It.IsAny<CancellationToken>()))
            .Returns(value: ReadHttpMessagesAsync(message: message));

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        using CancellationTokenSource stopped = new();
        stopped.Cancel();

        // When

        Func<Task> dispatch = () => service.ProcessHttpEventMessagesAsync(
            cancellationToken: stopped.Token);

        // Then

        await Assert.ThrowsAsync<ServiceException>(testCode: dispatch);

        eventService.Verify(expression: value => value.RecordStarted(
            name: message.EventName), times: Times.Once);

        eventService.Verify(expression: value => value.RecordFinished(
            name: message.EventName,
            succeeded: false), times: Times.Once);
    }

    [Fact]
    public async Task ShouldRecordFailedHandler()
    {
        // Given

        HttpEventMessage message = new() { EventName = "invoice", Data = "{}" };

        TaskCompletionSource<bool> finished = new(
            creationOptions: TaskCreationOptions.RunContinuationsAsynchronously);

        Mock<IHttpEventService> eventService = new();

        eventService.Setup(expression: value => value.GetMaxConcurrency())
            .Returns(value: 1);

        eventService.Setup(expression: value =>
                value.ReadAllHttpEventMessagesAsync(
                    cancellationToken: It.IsAny<CancellationToken>()))
            .Returns(value: ReadHttpMessagesAsync(message: message));

        eventService.Setup(expression: value =>
                value.DispatchHttpEventMessageAsync(
                    httpEventMessage: message))
            .Returns(value: ValueTask.FromException<bool>(
                exception: new NotSupportedException()));

        eventService.Setup(expression: value => value.RecordFinished(
                name: message.EventName,
                succeeded: false))
            .Callback(callback: () => finished.SetResult(result: true));

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        await service.ProcessHttpEventMessagesAsync(
            cancellationToken: CancellationToken.None);

        await finished.Task.WaitAsync(
            timeout: TimeSpan.FromSeconds(value: 5));

        // Then

        eventService.Verify(expression: value => value.RecordFinished(
            name: message.EventName,
            succeeded: false), times: Times.Once);
    }
}