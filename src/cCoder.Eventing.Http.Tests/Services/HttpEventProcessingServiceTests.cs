// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Models;
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
    public async Task ShouldMapAndSendSingleEventMessage()
    {
        // Given

        HttpEventMessage actualMessage = null;
        Mock<IHttpEventService> eventService = CreateEventServiceMock();

        eventService
            .Setup(expression: service => service.SendHttpEventMessageAsync(
                httpEventMessage: It.IsAny<HttpEventMessage>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .Callback<HttpEventMessage, CancellationToken>(
                action: (message, _) => actualMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        await service.RaiseEventAsync(
            name: "event",
            message: new EventMessage<FakePayload>
            {
                AuthInfo = new EventAuthInfo { SSOUserId = "user-123" },
                Data = new FakePayload { Value = "hello" }
            });

        // Then

        actualMessage.EventName
            .Should()
            .Be(expected: "event");

        actualMessage.SSOUserId
            .Should()
            .Be(expected: "user-123");

        actualMessage.Data
            .Should()
            .Be(expected: "serialized");
    }

    [Fact]
    public async Task ShouldSendEveryBulkEventMessage()
    {
        // Given

        Mock<IHttpEventService> eventService = CreateEventServiceMock();

        HttpEventProcessingService service = new(
            httpEventService: eventService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        await service.RaiseEventsAsync(
            name: "event",
            messages:
            [
                CreateEventMessage(),
                CreateEventMessage()
            ]);

        // Then

        eventService.Verify(
            expression: value => value.SendHttpEventMessageAsync(
                httpEventMessage: It.IsAny<HttpEventMessage>(),
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Exactly(callCount: 2));
    }

    private static Mock<IHttpEventService> CreateEventServiceMock()
    {
        Mock<IHttpEventService> eventService = new();

        eventService
            .Setup(expression: service => service.IsConfigured())
            .Returns(value: true);

        eventService
            .Setup(expression: service => service.Serialize(
                value: It.IsAny<object>()))
            .Returns(value: "serialized");

        eventService
            .Setup(expression: service => service.SendHttpEventMessageAsync(
                httpEventMessage: It.IsAny<HttpEventMessage>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .Returns(value: ValueTask.CompletedTask);

        return eventService;
    }

    private static EventMessage<FakePayload> CreateEventMessage() =>
        new()
        {
            AuthInfo = new EventAuthInfo(),
            Data = new FakePayload()
        };
}