// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Dependencies;
using cCoder.Eventing.AzureServiceBus.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Brokers;

public sealed partial class ServiceBusBrokerTests
{
    [Fact]
    public void CreateProcessor_ShouldApplyConfiguredMaxConcurrency()
    {
        // Given

        string inputName = "event-name";
        ServiceBusProcessorOptions actualOptions = null;
        Mock<ServiceBusProcessor> processorMock = new();
        Mock<ServiceBusClient> clientMock = new();

        clientMock
            .Setup(expression:client => client.CreateProcessor(
queueName: inputName,
options: It.IsAny<ServiceBusProcessorOptions>()))
            .Callback<string, ServiceBusProcessorOptions>(action:(_, options) => actualOptions = options)
            .Returns(value:processorMock.Object);

        ServiceBusBroker broker = new(
            clientMock.Object,
            new AzureServiceBusEventingConfiguration
            {
                MaxConcurrency = 4
            });

        // When

        ServiceBusProcessor actualProcessor = broker.CreateProcessor(name:inputName);

        // Then

        actualProcessor.Should()
            .BeSameAs(expected:processorMock.Object);

        actualOptions.Should()
            .NotBeNull();

        actualOptions.MaxConcurrentCalls.Should()
            .Be(expected:4);
    }

    [Fact]
    public void CreateProcessor_ShouldUseMinimumMaxConcurrencyOfOne()
    {
        // Given

        string inputName = "event-name";
        ServiceBusProcessorOptions actualOptions = null;
        Mock<ServiceBusProcessor> processorMock = new();
        Mock<ServiceBusClient> clientMock = new();

        clientMock
            .Setup(expression:client => client.CreateProcessor(
queueName: inputName,
options: It.IsAny<ServiceBusProcessorOptions>()))
            .Callback<string, ServiceBusProcessorOptions>(action:(_, options) => actualOptions = options)
            .Returns(value:processorMock.Object);

        ServiceBusBroker broker = new(
            clientMock.Object,
            new AzureServiceBusEventingConfiguration
            {
                MaxConcurrency = 0
            });

        // When

        broker.CreateProcessor(name:inputName);

        // Then

        actualOptions.Should()
            .NotBeNull();

        actualOptions.MaxConcurrentCalls.Should()
            .Be(expected:1);
    }
}