// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Processings;
using cCoder.Eventing.Models;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Exposures;

public partial class HttpEventHubTests
{
    [Fact]
    public async Task ShouldForwardEveryExposureOperation()
    {
        // Given

        const string eventName = "test-event";
        CancellationToken cancellationToken = new(canceled: false);
        EventMessage<FakePayload> message = new();
        EventMessage<FakePayload>[] messages = [message];
        HttpEventMessage transportMessage = new();

        Func<IServiceProvider, FakePayload, ValueTask> handler =
            (_, _) => ValueTask.CompletedTask;

        Mock<IHttpEventProcessingService> processingService = new();

        HttpEventHub eventHub = new(
            httpEventProcessingService: processingService.Object);

        // When

        eventHub.ListenToEvent(name: eventName, handler: handler);

        await eventHub.RaiseEventAsync(
            name: eventName,
            message: message,
            cancellationToken: cancellationToken);

        await eventHub.RaiseEventsAsync(
            name: eventName,
            messages: messages,
            cancellationToken: cancellationToken);

        await eventHub.ReceiveEventAsync(
            message: transportMessage,
            cancellationToken: cancellationToken);

        // Then

        processingService.Verify(
            expression: service => service.ListenToEvent(
                name: eventName,
                handler: handler),
            times: Times.Once);

        processingService.Verify(
            expression: service => service.RaiseEventAsync(
                name: eventName,
                message: message,
                cancellationToken: cancellationToken),
            times: Times.Once);

        processingService.Verify(
            expression: service => service.RaiseEventsAsync(
                name: eventName,
                messages: messages,
                cancellationToken: cancellationToken),
            times: Times.Once);

        processingService.Verify(
            expression: service => service.ReceiveEventAsync(
                message: transportMessage,
                cancellationToken: cancellationToken),
            times: Times.Once);
    }
}