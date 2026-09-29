// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Http;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Processings;
using cCoder.Eventing.Models;
using Moq;

namespace cCoder.Eventing.IntegrationTests.Tests;

public sealed partial class HttpEventHubExposureTests
{
    [Fact]
    public async Task ShouldForwardEveryHttpEventHubOperation()
    {
        // Given

        const string eventName = "test-event";
        CancellationToken cancellationToken = new(canceled: false);
        EventMessage<string> message = new();
        EventMessage<string>[] messages = [message];
        HttpEventMessage transportMessage = new();
        Func<IServiceProvider, string, ValueTask> handler = (_, _) => ValueTask.CompletedTask;
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
            httpEventMessage: transportMessage,
            cancellationToken: cancellationToken);

        // Then

        processingService.Verify(
            expression: service => service.ListenToEvent(name: eventName, handler: handler),
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
            expression: service => service.ReceiveHttpEventMessageAsync(
                httpEventMessage: transportMessage,
                cancellationToken: cancellationToken),
            times: Times.Once);
    }
}