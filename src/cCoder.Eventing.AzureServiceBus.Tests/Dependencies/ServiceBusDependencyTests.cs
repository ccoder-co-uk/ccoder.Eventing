// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Dependencies;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Transports;

public partial class ServiceBusTransportTests
{
    [Fact]
    public async Task ShouldReuseSenderAndDisposeTransportResources()
    {
        // Given

        const string eventName = "test-event";
        Mock<ServiceBusSender> sender = new();
        Mock<ServiceBusClient> client = new();

        client
            .Setup(expression: dependency => dependency.CreateSender(
                queueOrTopicName: eventName))
            .Returns(value: sender.Object);

        ServiceBusDependency dependency = new(
            maxConcurrency: 1,
            client: client.Object);

        BinaryData body = new(data: "payload");

        // When

        await dependency.SendAsync(
            name: eventName,
            body: body,
            messageId: "message-id-1");

        await dependency.SendAsync(
            name: eventName,
            body: body,
            messageId: "message-id-2");

        await dependency.DisposeAsync();

        // Then

        client.Verify(
            expression: transport => transport.CreateSender(
                queueOrTopicName: eventName),
            times: Times.Once);

        sender.Verify(
            expression: transport => transport.SendMessageAsync(
                message: It.IsAny<ServiceBusMessage>(),
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Exactly(callCount: 2));

        sender.Verify(
            expression: transport => transport.DisposeAsync(),
            times: Times.Once);

        client.Verify(
            expression: transport => transport.DisposeAsync(),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldReuseProcessorAndAttachHandlers()
    {
        // Given

        const string eventName = "test-event";

        Mock<ServiceBusProcessor> processor = new();
        Mock<ServiceBusClient> client = new();

        client
            .Setup(expression: dependency => dependency.CreateProcessor(
                queueName: eventName,
                options: It.IsAny<ServiceBusProcessorOptions>()))
            .Returns(value: processor.Object);

        ServiceBusDependency dependency = new(
            maxConcurrency: 0,
            client: client.Object);

        // When

        dependency.Listen(
            name: eventName,
            handler: _ => ValueTask.CompletedTask,
            errorHandler: _ => Task.CompletedTask);

        await dependency.DisposeAsync();

        // Then

        client.Verify(
            expression: transport => transport.CreateProcessor(
                queueName: eventName,
                options: It.Is<ServiceBusProcessorOptions>(match: options =>
                    options.MaxConcurrentCalls == 1)),
            times: Times.Once);

        processor.Verify(
            expression: transport => transport.StartProcessingAsync(
                cancellationToken: It.IsAny<CancellationToken>()),
            times: Times.Once);


    }
}