// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Models.Exceptions;
using cCoder.Eventing.Http.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public sealed partial class HttpEventServiceTests
{
    [Fact]
    public async Task ShouldForwardFoundationOperationsToBroker()
    {
        // Given

        HttpEventMessage message = CreateValidHttpEventMessage();
        Mock<IHttpEventBroker> broker = new();

        broker.Setup(expression: value => value.IsConfigured())
            .Returns(value: true);

        broker.Setup(expression: value => value.Serialize(
                value: It.IsAny<object>()))
            .Returns(value: "serialized");

        broker.Setup(expression: value => value.SelectMaxConcurrency())
            .Returns(value: 3);

        broker.Setup(expression: value => value.ReadAllAsync(
                cancellationToken: It.IsAny<CancellationToken>()))
            .Returns(value: ReadMessagesAsync(message: message));

        HttpEventService service = new(
            httpEventBroker: broker.Object,
            serviceProviderBroker: Mock.Of<IServiceProviderBroker>());

        // When

        bool configured = service.IsConfigured();
        string serialized = service.Serialize(value: new object());
        int maxConcurrency = service.GetMaxConcurrency();

        await service.SendHttpEventMessageAsync(httpEventMessage: message);
        await service.EnqueueHttpEventMessageAsync(httpEventMessage: message);

        List<HttpEventMessage> messages = [];

        await foreach (HttpEventMessage currentMessage in
            service.ReadAllHttpEventMessagesAsync(
                cancellationToken: CancellationToken.None))
        {
            messages.Add(item: currentMessage);
        }

        // Then

        configured
            .Should()
            .BeTrue();

        serialized
            .Should()
            .Be(expected: "serialized");

        maxConcurrency
            .Should()
            .Be(expected: 3);

        messages
            .Should()
            .ContainSingle()
            .Which
            .Should()
            .BeSameAs(expected: message);

        broker.Verify(
            expression: value => value.SendAsync(
                httpEventMessage: message,
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);

        broker.Verify(
            expression: value => value.EnqueueAsync(
                httpEventMessage: message,
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldRejectInvalidFoundationRequests()
    {
        // Given

        HttpEventService service = new(
            httpEventBroker: Mock.Of<IHttpEventBroker>(),
            serviceProviderBroker: Mock.Of<IServiceProviderBroker>());

        // When

        Action serializeNull = () => service.Serialize(value: null);

        Action listenWithoutName = () => service.ListenToEvent<object>(
            name: " ",
            handler: (_, _) => ValueTask.CompletedTask);

        Func<Task> sendNull = async () =>
            await service.SendHttpEventMessageAsync(httpEventMessage: null);

        Func<Task> enqueueNull = async () =>
            await service.EnqueueHttpEventMessageAsync(httpEventMessage: null);

        // Then

        serializeNull
            .Should()
            .Throw<ServiceValidationException>();

        listenWithoutName
            .Should()
            .Throw<ServiceValidationException>();

        await sendNull
            .Should()
            .ThrowAsync<ServiceValidationException>();

        await enqueueNull
            .Should()
            .ThrowAsync<ServiceValidationException>();
    }

    [Fact]
    public async Task ShouldRejectSendWhenHttpTransportIsNotConfigured()
    {
        // Given

        Mock<IHttpEventBroker> broker = new();

        broker.Setup(expression: value => value.IsConfigured())
            .Returns(value: false);

        HttpEventService service = new(
            httpEventBroker: broker.Object,
            serviceProviderBroker: Mock.Of<IServiceProviderBroker>());

        // When

        Func<Task> send = async () =>
            await service.SendHttpEventMessageAsync(
                httpEventMessage: CreateValidHttpEventMessage());

        // Then

        await send
            .Should()
            .ThrowAsync<ServiceDependencyException>();
    }

    [Fact]
    public async Task ShouldReturnNotHandledWhenNoHandlersMatch()
    {
        // Given

        Mock<IHttpEventBroker> broker = new();

        broker.Setup(expression: value => value.SelectSubscriptions(
                name: It.IsAny<string>()))
            .Returns(value: []);

        broker.Setup(expression: value =>
                value.SelectEventProviderConfigurations(
                    name: It.IsAny<string>()))
            .Returns(value: []);

        HttpEventService service = new(
            httpEventBroker: broker.Object,
            serviceProviderBroker: Mock.Of<IServiceProviderBroker>());

        // When

        bool handled = await service.DispatchHttpEventMessageAsync(
            httpEventMessage: CreateValidHttpEventMessage());

        // Then

        handled
            .Should()
            .BeFalse();
    }

    [Fact]
    public void ShouldWrapUnexpectedBrokerFailure()
    {
        // Given

        Mock<IHttpEventBroker> broker = new();

        broker.Setup(expression: value => value.IsConfigured())
            .Throws<NotSupportedException>();

        HttpEventService service = new(
            httpEventBroker: broker.Object,
            serviceProviderBroker: Mock.Of<IServiceProviderBroker>());

        // When

        Action readConfiguration = () => service.IsConfigured();

        // Then

        readConfiguration
            .Should()
            .Throw<ServiceException>();
    }

    [Fact]
    public void ShouldWrapInvalidBrokerOperationAsDependencyFailure()
    {
        // Given

        Mock<IHttpEventBroker> broker = new();

        broker.Setup(expression: value => value.Serialize(
                value: It.IsAny<object>()))
            .Throws<InvalidOperationException>();

        HttpEventService service = new(
            httpEventBroker: broker.Object,
            serviceProviderBroker: Mock.Of<IServiceProviderBroker>());

        // When

        Action serialize = () => service.Serialize(value: new object());

        // Then

        serialize
            .Should()
            .Throw<ServiceDependencyException>();
    }

    private static HttpEventMessage CreateValidHttpEventMessage() =>
        new()
        {
            EventName = "event",
            Data = "{}"
        };

    private static async IAsyncEnumerable<HttpEventMessage> ReadMessagesAsync(
        HttpEventMessage message)
    {
        yield return message;
        await Task.CompletedTask;
    }
}