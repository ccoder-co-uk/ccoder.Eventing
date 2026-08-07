// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Dependencies;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Models;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public partial class HttpEventProcessingServiceTests
{
    [Fact]
    public async Task ShouldForwardAllSingleEventOperations()
    {
        // Given

        const string eventName = "test-event";
        CancellationToken cancellationToken = new(canceled: false);
        EventMessage<FakePayload> message = new();
        HttpEventMessage transportMessage = new();

        Func<IServiceProvider, FakePayload, ValueTask> handler =
            (_, _) => ValueTask.CompletedTask;

        Mock<IHttpEventService> eventService = new();

        HttpEventProcessingServiceDependency processingService = new(
            httpEventService: eventService.Object);

        // When

        processingService.ListenToEvent(name: eventName, handler: handler);

        await processingService.RaiseEventAsync(
            name: eventName,
            message: message,
            cancellationToken: cancellationToken);

        await processingService.ReceiveEventAsync(
            message: transportMessage,
            cancellationToken: cancellationToken);

        // Then

        eventService.Verify(
            expression: service => service.ListenToEvent(
                name: eventName,
                handler: handler),
            times: Times.Once);

        eventService.Verify(
            expression: service => service.RaiseEventAsync(
                name: eventName,
                message: message,
                cancellationToken: cancellationToken),
            times: Times.Once);

        eventService.Verify(
            expression: service => service.ReceiveEventAsync(
                message: transportMessage,
                cancellationToken: cancellationToken),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldForwardEveryBulkEventAndAcceptNullArrays()
    {
        // Given

        const string eventName = "test-event";
        EventMessage<FakePayload>[] messages = [new(), new()];
        Mock<IHttpEventService> eventService = new();

        HttpEventProcessingServiceDependency processingService = new(
            httpEventService: eventService.Object);

        // When

        await processingService.RaiseEventsAsync(
            name: eventName,
            messages: messages);

        await processingService.RaiseEventsAsync<FakePayload>(
            name: eventName,
            messages: null);

        // Then

        eventService.Verify(
            expression: service => service.RaiseEventAsync(
                name: eventName,
                message: It.IsAny<EventMessage<FakePayload>>(),
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Exactly(callCount: 2));
    }
}