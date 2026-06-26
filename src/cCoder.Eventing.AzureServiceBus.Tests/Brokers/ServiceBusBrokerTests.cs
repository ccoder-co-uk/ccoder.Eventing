using Azure.Messaging.ServiceBus;
using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Brokers;

public sealed class ServiceBusBrokerTests
{
    [Fact]
    public void CreateProcessor_ShouldApplyConfiguredMaxConcurrency()
    {
        string inputName = "event-name";
        ServiceBusProcessorOptions actualOptions = null;
        Mock<ServiceBusProcessor> processorMock = new();
        Mock<ServiceBusClient> clientMock = new();

        clientMock
            .Setup(client => client.CreateProcessor(
                inputName,
                It.IsAny<ServiceBusProcessorOptions>()))
            .Callback<string, ServiceBusProcessorOptions>((_, options) => actualOptions = options)
            .Returns(processorMock.Object);

        ServiceBusBroker broker = new(
            clientMock.Object,
            new AzureServiceBusEventingConfiguration
            {
                MaxConcurrency = 4
            });

        ServiceBusProcessor actualProcessor = broker.CreateProcessor(inputName);

        actualProcessor.Should().BeSameAs(processorMock.Object);
        actualOptions.Should().NotBeNull();
        actualOptions.MaxConcurrentCalls.Should().Be(4);
    }

    [Fact]
    public void CreateProcessor_ShouldUseMinimumMaxConcurrencyOfOne()
    {
        string inputName = "event-name";
        ServiceBusProcessorOptions actualOptions = null;
        Mock<ServiceBusProcessor> processorMock = new();
        Mock<ServiceBusClient> clientMock = new();

        clientMock
            .Setup(client => client.CreateProcessor(
                inputName,
                It.IsAny<ServiceBusProcessorOptions>()))
            .Callback<string, ServiceBusProcessorOptions>((_, options) => actualOptions = options)
            .Returns(processorMock.Object);

        ServiceBusBroker broker = new(
            clientMock.Object,
            new AzureServiceBusEventingConfiguration
            {
                MaxConcurrency = 0
            });

        broker.CreateProcessor(inputName);

        actualOptions.Should().NotBeNull();
        actualOptions.MaxConcurrentCalls.Should().Be(1);
    }
}
