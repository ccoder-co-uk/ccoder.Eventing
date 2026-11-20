// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Authentication;
using System.Threading;
using System.Threading.Tasks;

using cCoder.Eventing.Apps.Controllers;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Models.Exceptions;
using cCoder.Eventing.Apps.Services.Orchestrations;
using cCoder.Eventing.Http.Brokers.Loggings;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Apps;

public partial class ChatControllerTests
{
    [Fact]
    public async Task ShouldAcceptValidChatRequests()
    {
        // Given

        ChatMessage expectedMessage = new();
        Mock<IChatOrchestrationService> manager = new();
        Mock<ILoggingBroker> loggingBroker = new();

        manager
            .Setup(expression: service => service.SendChatMessageAsync(
                chatMessage: It.IsAny<ChatMessage>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: expectedMessage);

        ChatController controller = CreateController(
            manager: manager.Object,
            loggingBroker: loggingBroker.Object);

        // When

        IActionResult result = await controller.Post(
            newChatMessageRequest: new ChatMessageRequest
            {
                User = "user",
                Text = "hello"
            },
            cancellationToken: default);

        // Then

        result
            .Should()
            .BeOfType<AcceptedResult>();

        manager.Verify(
            expression: service => service.SendChatMessageAsync(
                chatMessage: It.IsAny<ChatMessage>(),
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

    [Fact]
    public async Task ShouldRejectInvalidModelState()
    {
        // Given

        Mock<ILoggingBroker> loggingBroker = new();

        ChatController controller = CreateController(
            manager: Mock.Of<IChatOrchestrationService>(),
            loggingBroker: loggingBroker.Object);

        controller.ModelState.AddModelError(
            key: "Text",
            errorMessage: "Required");

        // When

        IActionResult result = await controller.Post(
            newChatMessageRequest: new ChatMessageRequest(),
            cancellationToken: default);

        // Then

        result
            .Should()
            .BeOfType<BadRequestObjectResult>();

        loggingBroker.Verify(
            expression: broker => broker.LogWarning(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Once);

        loggingBroker.Verify(
            expression: broker => broker.LogInformation(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);

        loggingBroker.VerifyNoOtherCalls();
    }

    [Theory]
    [MemberData(nameof(FailureCases))]
    public async Task ShouldTranslateChatFailures(
        Exception failure,
        int expectedStatusCode,
        string expectedResponse)
    {
        // Given

        Mock<IChatOrchestrationService> manager = new();
        Mock<ILoggingBroker> loggingBroker = new();

        manager
            .Setup(expression: service => service.SendChatMessageAsync(
                chatMessage: It.IsAny<ChatMessage>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception: failure);

        ChatController controller = CreateController(
            manager: manager.Object,
            loggingBroker: loggingBroker.Object);

        // When

        IActionResult result = await controller.Post(
            newChatMessageRequest: new ChatMessageRequest(),
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

        }
        else
        {
            loggingBroker.Verify(
                expression: broker => broker.LogWarning(
                    exception: failure,
                    message: It.IsAny<string>(),
                    args: It.IsAny<object[]>()),
                times: Times.Once);
        }

        loggingBroker.Verify(
            expression: broker => broker.LogInformation(
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Never);

        loggingBroker.VerifyNoOtherCalls();
    }

    private static ChatController CreateController(
        IChatOrchestrationService manager,
        ILoggingBroker loggingBroker)
    {
        return new(
                chatOrchestrationService: manager,
                configuration: new AppConfiguration
                {
                    EventingChat = new EventingChatConfiguration
                    {
                        AppName = "Test.App"
                    }
                },
                loggingBroker: loggingBroker);
    }

    public static IEnumerable<object[]> FailureCases() =>
        [
            [new ChatServiceValidationException(new ArgumentException()), 400, "The chat request is invalid."],
            [new AuthenticationException(), 401, "Authentication failed."],
            [new SecurityException(), 403, "The chat request is forbidden."],
            [new Exception("Sensitive detail."), 500, "The chat request failed."]
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