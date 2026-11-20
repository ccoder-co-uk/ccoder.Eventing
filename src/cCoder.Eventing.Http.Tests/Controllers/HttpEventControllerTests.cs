// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Authentication;
using System.Threading;

using System.Threading.Tasks;

using cCoder.Eventing.Http.Brokers.Loggings;
using cCoder.Eventing.Http.Controllers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Models.Exceptions;
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
        Mock<ILoggingBroker> loggingBroker = new();

        HttpEventController controller = new(
            httpEventProcessingService: eventProcessingService.Object,
            loggingBroker: loggingBroker.Object);

        // When

        IActionResult result = await controller.Post(
            newHttpEventMessage: message,
            cancellationToken: default);

        // Then

        result
            .Should()
            .BeOfType<AcceptedResult>();

        eventProcessingService.Verify(
            expression: service => service.ReceiveHttpEventMessageAsync(
                httpEventMessage: message,
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);

        loggingBroker.Verify(
            expression: broker => broker.LogInformation(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Once);

        VerifyNoFailureLogs(loggingBroker: loggingBroker);
        loggingBroker.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(FailureCases))]
    public async Task ShouldTranslateReceiveFailures(
        Exception failure,
        int expectedStatusCode,
        string expectedResponse)
    {
        // Given

        HttpEventMessage message = new();

        Mock<IHttpEventProcessingService> eventProcessingService = new();
        Mock<ILoggingBroker> loggingBroker = new();

        eventProcessingService
            .Setup(expression: service => service.ReceiveHttpEventMessageAsync(
                httpEventMessage: message,
                cancellationToken: It.IsAny<CancellationToken>()))
            .Returns(value: ValueTask.FromException(exception: failure));

        HttpEventController controller = new(
            httpEventProcessingService: eventProcessingService.Object,
            loggingBroker: loggingBroker.Object);

        // When

        IActionResult result = await controller.Post(
            newHttpEventMessage: message,
            cancellationToken: default);

        // Then

        ((IStatusCodeActionResult)result)
            .StatusCode
            .Should()
            .Be(expected: expectedStatusCode);

        ((ObjectResult)result).Value.Should()
            .Be(expected: expectedResponse);

        if (expectedStatusCode == 500)
        {
            loggingBroker.Verify(
                expression: broker => broker.LogError(
                    exception: failure,
                    message: It.IsAny<string>(),
                    args: It.IsAny<object[]>()),
                times: Times.Once);

            loggingBroker.Verify(
                expression: broker => broker.LogWarning(
                    exception: It.IsAny<Exception>(),
                    message: It.IsAny<string>(),
                    args: It.IsAny<object[]>()),
                times: Times.Never);

        }
        else
        {
            loggingBroker.Verify(
                expression: broker => broker.LogWarning(
                    exception: failure,
                    message: It.IsAny<string>(),
                    args: It.IsAny<object[]>()),
                times: Times.Once);

            loggingBroker.Verify(
                expression: broker => broker.LogError(
                    exception: It.IsAny<Exception>(),
                    message: It.IsAny<string>(),
                    args: It.IsAny<object[]>()),
                times: Times.Never);
        }

        loggingBroker.Verify(
            expression: broker => broker.LogInformation(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);

        loggingBroker.VerifyNoOtherCalls();
    }

    public static IEnumerable<object[]> FailureCases() =>
        [
            [new ServiceValidationException(new ArgumentException()), 400, "The event request is invalid."],
            [new AuthenticationException(), 401, "Authentication failed."],
            [new SecurityException(), 403, "The event request is forbidden."],
            [new Exception("Sensitive detail."), 500, "The event request failed."]
        ];

    private static void VerifyNoFailureLogs(Mock<ILoggingBroker> loggingBroker)
    {
        loggingBroker.Verify(
            expression: broker => broker.LogWarning(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);

        loggingBroker.Verify(
            expression: broker => broker.LogWarning(
                exception: It.IsAny<Exception>(),
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);

        loggingBroker.Verify(
            expression: broker => broker.LogError(
                exception: It.IsAny<Exception>(),
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);
    }
}