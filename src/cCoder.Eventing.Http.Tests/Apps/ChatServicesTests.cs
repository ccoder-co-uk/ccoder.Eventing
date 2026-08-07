// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Brokers;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Services.Foundations;
using cCoder.Eventing.Apps.Services.Orchestrations;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Apps;

public partial class ChatServicesTests
{
    [Fact]
    public async Task ShouldRaiseChatMessagesThroughBothEventTransports()
    {
        // Given

        ChatMessage message = new()
        {
            User = "user",
            Text = "hello"
        };

        Mock<IEventHub> eventHub = new();
        Mock<IHttpEventHub> httpEventHub = new();

        ChatEventService service = new(
            eventHub: eventHub.Object,
            httpEventHub: httpEventHub.Object);

        // When

        await service.RaiseChatMessageAsync(chatMessage: message);

        // Then

        eventHub.Verify(
            expression: hub => hub.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: It.Is<EventMessage<ChatMessage>>(match: eventMessage =>
                    eventMessage.Data == message)),
            times: Times.Once);

        httpEventHub.Verify(
            expression: hub => hub.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: It.Is<EventMessage<ChatMessage>>(match: eventMessage =>
                    eventMessage.Data == message),
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldValidateChatEventRequests()
    {
        // Given

        ChatEventService service = new(
            eventHub: Mock.Of<IEventHub>(),
            httpEventHub: Mock.Of<IHttpEventHub>());

        CancellationToken canceledToken = new(canceled: true);

        // When

        Exception nullFailure = await Record.ExceptionAsync(
            testCode: async () => await service.RaiseChatMessageAsync(
                chatMessage: null));

        Exception cancellationFailure = await Record.ExceptionAsync(
            testCode: async () => await service.RaiseChatMessageAsync(
                chatMessage: new ChatMessage(),
                cancellationToken: canceledToken));

        // Then

        nullFailure
            .Should()
            .NotBeNull();

        cancellationFailure
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ShouldSendAndValidateChatNotifications()
    {
        // Given

        ChatMessage message = new();
        Mock<IChatHubBroker> broker = new();
        ChatNotificationService service = new(chatHubBroker: broker.Object);

        // When

        await service.SendChatMessageAsync(chatMessage: message);

        Exception failure = await Record.ExceptionAsync(
            testCode: async () => await service.SendChatMessageAsync(
                chatMessage: null));

        // Then

        broker.Verify(
            expression: dependency => dependency.SendChatMessageAsync(
                chatMessage: message),
            times: Times.Once);

        failure
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ShouldNormalizeAndRaiseOutgoingChatMessages()
    {
        // Given

        ChatMessage message = new()
        {
            User = "  ",
            Text = " hello "
        };

        Mock<IChatEventService> eventService = new();

        ChatOrchestrationService service = new(
            chatEventService: eventService.Object,
            chatNotificationService: Mock.Of<IChatNotificationService>());

        // When

        ChatMessage result = await service.SendChatMessageAsync(
            newChatMessage: message);

        // Then

        result.User
            .Should()
            .Be(expected: "Guest");

        result.Text
            .Should()
            .Be(expected: "hello");

        result.Id
            .Should()
            .NotBeEmpty();

        result.CreatedOn
            .Should()
            .NotBe(unexpected: default);

        eventService.Verify(
            expression: dependency => dependency.RaiseChatMessageAsync(
                chatMessage: message,
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldPreserveTrimmedUserAndValidateOutgoingMessages()
    {
        // Given

        ChatOrchestrationService service = new(
            chatEventService: Mock.Of<IChatEventService>(),
            chatNotificationService: Mock.Of<IChatNotificationService>());

        ChatMessage namedMessage = new()
        {
            User = " user ",
            Text = "hello"
        };

        // When

        ChatMessage result = await service.SendChatMessageAsync(
            newChatMessage: namedMessage);

        Exception nullFailure = await Record.ExceptionAsync(
            testCode: async () => await service.SendChatMessageAsync(
                newChatMessage: null));

        Exception textFailure = await Record.ExceptionAsync(
            testCode: async () => await service.SendChatMessageAsync(
                newChatMessage: new ChatMessage()));

        Exception cancellationFailure = await Record.ExceptionAsync(
            testCode: async () => await service.SendChatMessageAsync(
                newChatMessage: namedMessage,
                cancellationToken: new CancellationToken(canceled: true)));

        // Then

        result.User
            .Should()
            .Be(expected: "user");

        nullFailure
            .Should()
            .NotBeNull();

        textFailure
            .Should()
            .NotBeNull();

        cancellationFailure
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ShouldForwardAndValidateIncomingChatMessages()
    {
        // Given

        ChatMessage message = new();
        Mock<IChatNotificationService> notificationService = new();

        ChatOrchestrationService service = new(
            chatEventService: Mock.Of<IChatEventService>(),
            chatNotificationService: notificationService.Object);

        // When

        await service.ReceiveChatMessageAsync(chatMessage: message);

        Exception failure = await Record.ExceptionAsync(
            testCode: async () => await service.ReceiveChatMessageAsync(
                chatMessage: null));

        // Then

        notificationService.Verify(
            expression: dependency => dependency.SendChatMessageAsync(
                chatMessage: message),
            times: Times.Once);

        failure
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ShouldTranslateChatServiceDependencyFailures()
    {
        // Given

        Mock<IEventHub> eventHub = new();

        eventHub
            .Setup(expression: hub => hub.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: It.IsAny<EventMessage<ChatMessage>>()))
            .ThrowsAsync(exception: new Exception());

        ChatEventService eventService = new(
            eventHub: eventHub.Object,
            httpEventHub: Mock.Of<IHttpEventHub>());

        Mock<IChatHubBroker> broker = new();

        broker
            .Setup(expression: dependency => dependency.SendChatMessageAsync(
                chatMessage: It.IsAny<ChatMessage>()))
            .ThrowsAsync(exception: new Exception());

        ChatNotificationService notificationService = new(
            chatHubBroker: broker.Object);

        // When

        Exception eventFailure = await Record.ExceptionAsync(
            testCode: async () => await eventService.RaiseChatMessageAsync(
                chatMessage: new ChatMessage()));

        Exception notificationFailure = await Record.ExceptionAsync(
            testCode: async () => await notificationService.SendChatMessageAsync(
                chatMessage: new ChatMessage()));

        // Then

        eventFailure
            .Should()
            .NotBeNull();

        notificationFailure
            .Should()
            .NotBeNull();
    }
}