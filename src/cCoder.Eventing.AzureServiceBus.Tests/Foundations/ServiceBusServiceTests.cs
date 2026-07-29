// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Brokers;
using cCoder.Eventing.AzureServiceBus.Services.Foundations;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Eventing.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    private readonly Mock<IServiceBusBroker> serviceBusBrokerMock = new();
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock = new();
    private readonly Mock<ILogger<ServiceBusService>> loggerMock = new();
    private IServiceBusService ServiceBusService =>
        new ServiceBusService(
            serviceBusBroker: serviceBusBrokerMock.Object,
            serviceProviderBroker: serviceProviderBrokerMock.Object,
            log: loggerMock.Object);
}