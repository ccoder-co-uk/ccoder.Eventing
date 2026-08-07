// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Dependencies;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text.Json;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public partial class HttpEventServiceValidationTests
{
    [Fact]
    public async Task ShouldValidateEveryOutgoingMessageRequirement()
    {
        // Given

        IHttpEventService eventService = CreateEventService();

        EventMessage<FakePayload> validMessage = new()
        {
            AuthInfo = new EventAuthInfo(),
            Data = new FakePayload()
        };

        EventMessage<FakePayload> missingData = new()
        {
            AuthInfo = new EventAuthInfo()
        };

        EventMessage<FakePayload> missingAuth = new()
        {
            Data = new FakePayload()
        };

        Func<Task>[] invalidOperations =
        [
            async () => await eventService.RaiseEventAsync(
                name: null,
                message: validMessage),
            async () => await eventService.RaiseEventAsync<FakePayload>(
                name: "event",
                message: null),
            async () => await eventService.RaiseEventAsync(
                name: "event",
                message: missingData),
            async () => await eventService.RaiseEventAsync(
                name: "event",
                message: missingAuth)
        ];

        // When

        List<Exception> failures = [];

        foreach (Func<Task> operation in invalidOperations)
        {
            failures.Add(item: await Record.ExceptionAsync(testCode: operation));
        }

        // Then

        failures
            .Should()
            .OnlyContain(predicate: failure =>
                failure is InvalidOperationException);
    }

    [Fact]
    public async Task ShouldValidateAndEnqueueIncomingMessages()
    {
        // Given

        Mock<IHttpEventQueue> eventQueue = new();
        IHttpEventService eventService = CreateEventService(eventQueue: eventQueue);

        HttpEventMessage validMessage = new()
        {
            EventName = "event",
            Data = "{}"
        };

        HttpEventMessage missingName = new()
        {
            Data = "{}"
        };

        HttpEventMessage missingData = new()
        {
            EventName = "event"
        };

        // When

        await eventService.ReceiveEventAsync(message: validMessage);

        Exception missingMessageFailure = await Record.ExceptionAsync(
            testCode: async () => await eventService.ReceiveEventAsync(
                message: null));

        Exception missingNameFailure = await Record.ExceptionAsync(
            testCode: async () => await eventService.ReceiveEventAsync(
                message: missingName));

        Exception missingDataFailure = await Record.ExceptionAsync(
            testCode: async () => await eventService.ReceiveEventAsync(
                message: missingData));

        // Then

        eventQueue.Verify(
            expression: queue => queue.EnqueueAsync(
                message: validMessage,
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);

        missingMessageFailure
            .Should()
            .BeOfType<InvalidOperationException>();

        missingNameFailure
            .Should()
            .BeOfType<InvalidOperationException>();

        missingDataFailure
            .Should()
            .BeOfType<InvalidOperationException>();
    }

    [Fact]
    public void ShouldForwardSubscriptions()
    {
        // Given

        const string eventName = "event";

        Func<IServiceProvider, FakePayload, ValueTask> handler =
            (_, _) => ValueTask.CompletedTask;

        Mock<IHttpEventHandlerRegistry> registry = new();
        IHttpEventService eventService = CreateEventService(registry: registry);

        // When

        eventService.ListenToEvent(name: eventName, handler: handler);

        // Then

        registry.Verify(
            expression: service => service.ListenToEvent(
                name: eventName,
                handler: handler),
            times: Times.Once);
    }

    private static IHttpEventService CreateEventService(
        Mock<IHttpEventQueue> eventQueue = null,
        Mock<IHttpEventHandlerRegistry> registry = null) =>
        new HttpEventServiceDependency(
            httpEventBroker: Mock.Of<IHttpEventBroker>(),
            httpEventQueue: (eventQueue ?? new Mock<IHttpEventQueue>()).Object,
            eventHandlerRegistry: (registry ??
                new Mock<IHttpEventHandlerRegistry>()).Object,
            options: new HttpEventingOptions
            {
                JsonSerializerOptions =
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)
            },
            log: NullLogger<HttpEventServiceDependency>.Instance);
}