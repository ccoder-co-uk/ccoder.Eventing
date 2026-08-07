// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Controllers;
using cCoder.Eventing.Http.Models;
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
        Mock<IHttpEventHub> eventHub = new();
        HttpEventController controller = new(httpEventHub: eventHub.Object);

        // When

        IActionResult result = await controller.Post(
            newMessage: message,
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

        Mock<IHttpEventHub> eventHub = new();

        eventHub
            .Setup(expression: hub => hub.ReceiveEventAsync(
                message: message,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception: failure);

        HttpEventController controller = new(httpEventHub: eventHub.Object);

        // When

        IActionResult result = await controller.Post(
            newMessage: message,
            cancellationToken: default);

        // Then

        int expectedStatusCode = isValidationFailure ? 400 : 500;

        ((IStatusCodeActionResult)result)
            .StatusCode
            .Should()
            .Be(expected: expectedStatusCode);
    }
}