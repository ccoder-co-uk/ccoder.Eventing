// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Models.Exceptions;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public sealed partial class HttpEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldForwardSubscriptionAndIncomingMessage()
    {
        // Given

        HttpEventMessage message = new()
        {
            EventName = "event",
            Data = "{}"
        };

        Func<IServiceProvider, FakePayload, ValueTask> handler =
            (_, _) => ValueTask.CompletedTask;

        Mock<IHttpEventService> eventService = new();

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        service.ListenToEvent(name: "event", handler: handler);
        await service.ReceiveHttpEventMessageAsync(httpEventMessage: message);

        // Then

        eventService.Verify(
            expression: value => value.ListenToEvent(
                name: "event",
                handler: handler),
            times: Times.Once);

        eventService.Verify(
            expression: value => value.EnqueueHttpEventMessageAsync(
                httpEventMessage: message,
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldRejectInvalidProcessingRequests()
    {
        // Given

        Mock<IHttpEventService> eventService = CreateEventServiceMock();

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        EventMessage<FakePayload> message = CreateEventMessage();
        EventMessage<FakePayload> messageWithoutData = CreateEventMessage();
        messageWithoutData.Data = null;

        EventMessage<FakePayload> messageWithoutAuth = CreateEventMessage();
        messageWithoutAuth.AuthInfo = null;

        // When

        Func<Task> raiseWithoutName = async () =>
            await service.RaiseEventAsync(name: " ", message: message);

        Func<Task> raiseWithoutData = async () =>
            await service.RaiseEventAsync(
                name: "event",
                message: messageWithoutData);

        Func<Task> raiseWithoutAuth = async () =>
            await service.RaiseEventAsync(
                name: "event",
                message: messageWithoutAuth);

        Action listenWithoutHandler = () =>
            service.ListenToEvent<FakePayload>(
                name: "event",
                handler: null);

        // Then

        await raiseWithoutName.Should()
            .ThrowAsync<ServiceValidationException>();

        await raiseWithoutData.Should()
            .ThrowAsync<ServiceValidationException>();

        await raiseWithoutAuth.Should()
            .ThrowAsync<ServiceValidationException>();

        listenWithoutHandler.Should()
            .Throw<ServiceValidationException>();
    }

    [Fact]
    public async Task ShouldRejectRaiseWhenHttpTransportIsNotConfigured()
    {
        // Given

        Mock<IHttpEventService> eventService = CreateEventServiceMock();

        eventService.Setup(expression: value => value.IsConfigured())
            .Returns(value: false);

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        Func<Task> raise = async () =>
            await service.RaiseEventAsync(
                name: "event",
                message: CreateEventMessage());

        // Then

        await raise
            .Should()
            .ThrowAsync<ServiceDependencyException>();
    }

    [Fact]
    public void ShouldWrapUnexpectedProcessingDependencyFailure()
    {
        // Given

        Mock<IHttpEventService> eventService = new();

        eventService.Setup(expression: value => value.ListenToEvent(
                name: It.IsAny<string>(),
                handler: It.IsAny<
                    Func<IServiceProvider, FakePayload, ValueTask>>()))
            .Throws<NotSupportedException>();

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        Action listen = () => service.ListenToEvent<FakePayload>(
            name: "event",
            handler: (_, _) => ValueTask.CompletedTask);

        // Then

        listen
            .Should()
            .Throw<ServiceException>();
    }

    [Fact]
    public async Task ShouldProcessQueuedMessageAndReportMissingHandler()
    {
        // Given

        HttpEventMessage message = new()
        {
            EventName = "event",
            Data = "{}"
        };

        TaskCompletionSource<bool> dispatchObserved = new(
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
            .Callback(callback: () => dispatchObserved.SetResult(result: true))
            .Returns(value: ValueTask.FromResult(result: false));

        Mock<ILoggingBroker> loggingBroker = new();

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: loggingBroker.Object);

        // When

        await service.ProcessHttpEventMessagesAsync(
            cancellationToken: CancellationToken.None);

        await dispatchObserved.Task.WaitAsync(
            timeout: TimeSpan.FromSeconds(value: 5));

        // Then

        loggingBroker.Verify(
            expression: value => value.LogError(
                exception: null,
                message: "HTTP event {EventName} was received but no matching handlers were registered.",
                args: It.Is<object[]>(match: arguments =>
                    arguments.Length == 1
                    && Equals(
                        objA: arguments[0],
                        objB: message.EventName))),
            times: Times.Once);
    }

    private static async IAsyncEnumerable<HttpEventMessage>
        ReadHttpMessagesAsync(HttpEventMessage message)
    {
        yield return message;
        await Task.CompletedTask;
    }
}