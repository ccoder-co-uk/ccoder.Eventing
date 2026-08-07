// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Dependencies;
using cCoder.Eventing.AzureServiceBus.Models;
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
        AzureServiceBusEventingConfiguration configuration = new();
        Mock<ServiceBusSender> sender = new();
        Mock<ServiceBusClient> client = new();

        client
            .Setup(expression: dependency => dependency.CreateSender(
                queueOrTopicName: eventName))
            .Returns(value: sender.Object);

        ServiceBusDependency dependency = new(
            configuration: configuration,
            client: client.Object);

        ServiceBusEventMessage<FakeObject> message = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo(),
            Data = new FakeObject()
        };

        // When

        await dependency.SendAsync(name: eventName, eventMessage: message);
        await dependency.SendAsync(name: eventName, eventMessage: message);
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

        AzureServiceBusEventingConfiguration configuration = new()
        {
            MaxConcurrency = 0
        };

        Mock<ServiceBusProcessor> processor = new();
        Mock<ServiceBusClient> client = new();

        client
            .Setup(expression: dependency => dependency.CreateProcessor(
                queueName: eventName,
                options: It.IsAny<ServiceBusProcessorOptions>()))
            .Returns(value: processor.Object);

        ServiceBusDependency dependency = new(
            configuration: configuration,
            client: client.Object);

        // When

        dependency.Listen<FakeObject>(
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