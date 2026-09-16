// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Exposures;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services.Orchestrations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Apps;

public sealed partial class ChatManagerTests
{
    [Fact]
    public async Task ShouldForwardEveryFacadeOperationToOrchestrationService()
    {
        // Given

        ChatMessage inputMessage = new();
        ChatMessage expectedMessage = new();
        Mock<IChatOrchestrationService> orchestrationService = new();

        orchestrationService
            .Setup(expression: service => service.SendChatMessageAsync(
                chatMessage: inputMessage,
                cancellationToken: It.IsAny<CancellationToken>()))
            .ReturnsAsync(value: expectedMessage);

        ChatManager manager = new(
            chatOrchestrationService: orchestrationService.Object);

        // When

        ChatMessage actualMessage = await manager.SendChatMessageAsync(
            chatMessage: inputMessage);

        await manager.ReceiveChatMessageAsync(chatMessage: inputMessage);

        // Then

        actualMessage
            .Should()
            .BeSameAs(expected: expectedMessage);

        orchestrationService.Verify(
            expression: service => service.ReceiveChatMessageAsync(
                chatMessage: inputMessage),
            times: Times.Once);
    }
}