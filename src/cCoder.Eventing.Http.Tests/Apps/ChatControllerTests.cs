// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Controllers;
using cCoder.Eventing.Apps.Exposures;
using cCoder.Eventing.Apps.Models;
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
        Mock<IChatManager> manager = new();

        manager
            .Setup(expression: service => service.SendChatMessageAsync(
                newChatMessage: It.IsAny<ChatMessage>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: expectedMessage);

        ChatController controller = CreateController(manager: manager.Object);

        // When

        IActionResult result = await controller.Post(
            newRequest: new ChatMessageRequest
            {
                User = "user",
                Text = "hello"
            },
            cancellationToken: default);

        // Then

        result
            .Should()
            .BeOfType<AcceptedResult>();
    }

    [Fact]
    public async Task ShouldRejectInvalidModelState()
    {
        // Given

        ChatController controller = CreateController(
            manager: Mock.Of<IChatManager>());

        controller.ModelState.AddModelError(
            key: "Text",
            errorMessage: "Required");

        // When

        IActionResult result = await controller.Post(
            newRequest: new ChatMessageRequest(),
            cancellationToken: default);

        // Then

        result
            .Should()
            .BeOfType<BadRequestObjectResult>();
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task ShouldTranslateChatFailures(bool isValidationFailure)
    {
        // Given

        Exception failure = isValidationFailure
            ? new InvalidOperationException()
            : new Exception();

        Mock<IChatManager> manager = new();

        manager
            .Setup(expression: service => service.SendChatMessageAsync(
                newChatMessage: It.IsAny<ChatMessage>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception: failure);

        ChatController controller = CreateController(manager: manager.Object);

        // When

        IActionResult result = await controller.Post(
            newRequest: new ChatMessageRequest(),
            cancellationToken: default);

        // Then

        int expectedStatusCode = isValidationFailure ? 400 : 500;

        ((IStatusCodeActionResult)result)
            .StatusCode
            .Should()
            .Be(expected: expectedStatusCode);
    }

    private static ChatController CreateController(IChatManager manager) =>
        new(
            chatOrchestrationService: manager,
            configuration: new EventingAppCommonConfiguration
            {
                Eventing = new EventingAppConfiguration
                {
                    AppName = "Test.App"
                }
            },
            loggingBroker: Mock.Of<ILoggingBroker>());
}