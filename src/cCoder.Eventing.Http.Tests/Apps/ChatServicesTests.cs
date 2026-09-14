// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Brokers;
using cCoder.Eventing.Apps.Models;
using cCoder.Eventing.Apps.Models.Exceptions;
using cCoder.Eventing.Apps.Services.Foundations;
using cCoder.Eventing.Apps.Services.Orchestrations;
using cCoder.Eventing.Apps.Services.Processings;
using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Apps;

public partial class ChatServicesTests
{
    [Fact]
    public async Task ShouldProcessChatMessageThroughLocalThenHttpFoundations()
    {
        // Given

        ChatMessage inputMessage = new()
        {
            User = "user",
            Text = "hello"
        };

        CancellationToken inputCancellationToken = new(canceled: false);
        Mock<IChatEventTransportService> localEventServiceMock = new();
        Mock<IChatEventTransportService> httpEventServiceMock = new();
        MockSequence sequence = new();
        EventMessage<ChatMessage> localEventMessage = null;

        localEventServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.RaiseChatMessageAsync(
                eventMessage: It.IsAny<EventMessage<ChatMessage>>(),
                cancellationToken: inputCancellationToken))
            .Callback<EventMessage<ChatMessage>, CancellationToken>(
                action: (message, _) => localEventMessage = message)
            .Returns(value: ValueTask.CompletedTask);

        httpEventServiceMock
            .InSequence(sequence: sequence)
            .Setup(expression: service => service.RaiseChatMessageAsync(
                eventMessage: It.Is<EventMessage<ChatMessage>>(match: message =>
                    ReferenceEquals(objA: message, objB: localEventMessage)),
                cancellationToken: inputCancellationToken))
            .Returns(value: ValueTask.CompletedTask);

        ChatEventTransportProcessingService service = new(
            chatEventTransportServices:
            [
                localEventServiceMock.Object,
                httpEventServiceMock.Object
            ]);

        // When

        await service.RaiseChatMessageAsync(
            chatMessage: inputMessage,
            cancellationToken: inputCancellationToken);

        // Then

        localEventServiceMock.VerifyAll();
        httpEventServiceMock.VerifyAll();

        localEventMessage.Data
            .Should()
            .BeSameAs(expected: inputMessage);

        localEventMessage.AuthInfo.SSOUserId
            .Should()
            .Be(expected: inputMessage.User);
    }

    [Fact]
    public async Task ShouldRaiseChatMessagesThroughBothEventTransports()
    {
        // Given

        ChatMessage message = new()
        {
            User = "user",
            Text = "hello"
        };

        Mock<IChatEventBroker> eventHub = new();
        Mock<IChatHttpEventBroker> httpEventHub = new();

        ChatLocalEventService localEventService = new(
            chatEventBroker: eventHub.Object);

        ChatHttpEventService httpEventService = new(
            chatHttpEventBroker: httpEventHub.Object);

        EventMessage<ChatMessage> inputEventMessage = new()
        {
            AuthInfo = new EventAuthInfo
            {
                SSOUserId = message.User
            },
            Data = message
        };

        // When

        await localEventService.RaiseChatMessageAsync(
            eventMessage: inputEventMessage);

        await httpEventService.RaiseChatMessageAsync(
            eventMessage: inputEventMessage);

        // Then

        eventHub.Verify(
            expression: hub => hub.RaiseChatMessageAsync(
                name: It.IsAny<string>(),
                message: It.Is<EventMessage<ChatMessage>>(match: actualMessage =>
                    ReferenceEquals(
                        objA: actualMessage,
                        objB: inputEventMessage))),
            times: Times.Once);

        httpEventHub.Verify(
            expression: hub => hub.RaiseChatMessageAsync(
                name: It.IsAny<string>(),
                message: It.Is<EventMessage<ChatMessage>>(match: actualMessage =>
                    ReferenceEquals(
                        objA: actualMessage,
                        objB: inputEventMessage)),
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldValidateAndTranslateLocalChatTransportFailures()
    {
        // Given

        Mock<IChatEventBroker> brokerMock = new();
        ChatLocalEventService service = new(chatEventBroker: brokerMock.Object);

        EventMessage<ChatMessage> inputMessage = new()
        {
            Data = new ChatMessage()
        };

        InvalidOperationException dependencyException = new(
            message: "dependency failure");

        brokerMock
            .Setup(expression: broker => broker.RaiseChatMessageAsync(
                name: It.IsAny<string>(),
                message: It.IsAny<EventMessage<ChatMessage>>()))
            .ThrowsAsync(exception: dependencyException);

        // When

        Func<Task> nullMessageTask = async () =>
            await service.RaiseChatMessageAsync(eventMessage: null);

        Func<Task> canceledTask = async () =>
            await service.RaiseChatMessageAsync(
                eventMessage: inputMessage,
                cancellationToken: new CancellationToken(canceled: true));

        Func<Task> dependencyTask = async () =>
            await service.RaiseChatMessageAsync(eventMessage: inputMessage);

        // Then

        await Assert.ThrowsAsync<ChatServiceValidationException>(
            testCode: nullMessageTask);

        await Assert.ThrowsAsync<ChatServiceException>(
            testCode: canceledTask);

        ChatServiceDependencyException actualException =
            await Assert.ThrowsAsync<ChatServiceDependencyException>(
                testCode: dependencyTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public async Task ShouldValidateAndTranslateHttpChatTransportFailures()
    {
        // Given

        Mock<IChatHttpEventBroker> brokerMock = new();
        ChatHttpEventService service = new(chatHttpEventBroker: brokerMock.Object);

        EventMessage<ChatMessage> inputMessage = new()
        {
            Data = new ChatMessage()
        };

        InvalidOperationException dependencyException = new(
            message: "dependency failure");

        brokerMock
            .Setup(expression: broker => broker.RaiseChatMessageAsync(
                name: It.IsAny<string>(),
                message: It.IsAny<EventMessage<ChatMessage>>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception: dependencyException);

        // When

        Func<Task> nullMessageTask = async () =>
            await service.RaiseChatMessageAsync(eventMessage: null);

        Func<Task> canceledTask = async () =>
            await service.RaiseChatMessageAsync(
                eventMessage: inputMessage,
                cancellationToken: new CancellationToken(canceled: true));

        Func<Task> dependencyTask = async () =>
            await service.RaiseChatMessageAsync(eventMessage: inputMessage);

        // Then

        await Assert.ThrowsAsync<ChatServiceValidationException>(
            testCode: nullMessageTask);

        await Assert.ThrowsAsync<ChatServiceException>(
            testCode: canceledTask);

        ChatServiceDependencyException actualException =
            await Assert.ThrowsAsync<ChatServiceDependencyException>(
                testCode: dependencyTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public async Task ShouldValidateChatEventRequests()
    {
        // Given

        ChatEventTransportProcessingService service = new(
            chatEventTransportServices:
            [
                Mock.Of<IChatEventTransportService>()
            ]);

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
    public async Task ShouldProcessChatNotificationThroughFoundation()
    {
        // Given

        ChatMessage inputMessage = new();
        Mock<IChatNotificationService> notificationServiceMock = new();

        ChatNotificationProcessingService service = new(
            chatNotificationService: notificationServiceMock.Object);

        // When

        await service.SendChatMessageAsync(chatMessage: inputMessage);

        // Then

        notificationServiceMock.Verify(
            expression: dependency => dependency.SendChatMessageAsync(
                chatMessage: inputMessage),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldTranslateChatNotificationProcessingFailure()
    {
        // Given

        InvalidOperationException dependencyException = new(
            message: "dependency failure");

        Mock<IChatNotificationService> notificationServiceMock = new();

        notificationServiceMock
            .Setup(expression: dependency => dependency.SendChatMessageAsync(
                chatMessage: It.IsAny<ChatMessage>()))
            .ThrowsAsync(exception: dependencyException);

        ChatNotificationProcessingService service = new(
            chatNotificationService: notificationServiceMock.Object);

        // When

        Func<Task> sendChatMessageTask = async () =>
            await service.SendChatMessageAsync(chatMessage: new ChatMessage());

        // Then

        ChatServiceDependencyException actualException =
            await Assert.ThrowsAsync<ChatServiceDependencyException>(
                testCode: sendChatMessageTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public async Task ShouldValidateChatNotificationProcessingRequest()
    {
        // Given

        ChatNotificationProcessingService service = new(
            chatNotificationService: Mock.Of<IChatNotificationService>());

        // When

        Func<Task> sendChatMessageTask = async () =>
            await service.SendChatMessageAsync(chatMessage: null);

        // Then

        await Assert.ThrowsAsync<ChatServiceValidationException>(
            testCode: sendChatMessageTask);
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

        Mock<IChatEventTransportProcessingService> eventService = new();

        ChatOrchestrationService service = new(
            chatEventService: eventService.Object,
            chatNotificationService: Mock.Of<IChatNotificationProcessingService>());

        // When

        ChatMessage result = await service.SendChatMessageAsync(
            chatMessage: message);

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
            chatEventService: Mock.Of<IChatEventTransportProcessingService>(),
            chatNotificationService: Mock.Of<IChatNotificationProcessingService>());

        ChatMessage namedMessage = new()
        {
            User = " user ",
            Text = "hello"
        };

        // When

        ChatMessage result = await service.SendChatMessageAsync(
            chatMessage: namedMessage);

        Exception nullFailure = await Record.ExceptionAsync(
            testCode: async () => await service.SendChatMessageAsync(
                chatMessage: null));

        Exception textFailure = await Record.ExceptionAsync(
            testCode: async () => await service.SendChatMessageAsync(
                chatMessage: new ChatMessage()));

        Exception cancellationFailure = await Record.ExceptionAsync(
            testCode: async () => await service.SendChatMessageAsync(
                chatMessage: namedMessage,
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
        Mock<IChatNotificationProcessingService> notificationService = new();

        ChatOrchestrationService service = new(
            chatEventService: Mock.Of<IChatEventTransportProcessingService>(),
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

        Mock<IChatEventTransportService> eventServiceMock = new();

        eventServiceMock
            .Setup(expression: service => service.RaiseChatMessageAsync(
                eventMessage: It.IsAny<EventMessage<ChatMessage>>(),
                cancellationToken: It.IsAny<CancellationToken>()))
            .ThrowsAsync(exception: new Exception());

        ChatEventTransportProcessingService eventService = new(
            chatEventTransportServices:
            [
                eventServiceMock.Object
            ]);

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