// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Controllers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Processings;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Controllers;

public partial class HttpEventControllerTests
{
    [Fact]
    public async Task ShouldReturnAcceptedWhenMessageIsReceived()
    {
        // Given

        HttpEventMessage message = new();
        Mock<IHttpEventProcessingService> eventProcessingService = new();

        HttpEventController controller = new(
            httpEventProcessingService: eventProcessingService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        IActionResult result = await controller.Post(
            newHttpEventMessage: message,
            cancellationToken: default);

        // Then

        result
            .Should()
            .BeOfType<AcceptedResult>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ShouldTranslateReceiveFailures(bool isValidationFailure)
    {
        // Given

        HttpEventMessage message = new();

        Exception failure = isValidationFailure
            ? new InvalidOperationException()
            : new Exception();

        Mock<IHttpEventProcessingService> eventProcessingService = new();

        eventProcessingService
            .Setup(expression: service => service.ReceiveHttpEventMessageAsync(
                httpEventMessage: message,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception: failure);

        HttpEventController controller = new(
            httpEventProcessingService: eventProcessingService.Object,
            loggingBroker: Mock.Of<ILoggingBroker>());

        // When

        IActionResult result = await controller.Post(
            newHttpEventMessage: message,
            cancellationToken: default);

        // Then

        int expectedStatusCode = isValidationFailure ? 400 : 500;

        ((IStatusCodeActionResult)result)
            .StatusCode
            .Should()
            .Be(expected: expectedStatusCode);
    }
}