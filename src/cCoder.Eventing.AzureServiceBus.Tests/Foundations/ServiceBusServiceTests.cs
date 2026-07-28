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
    private readonly Mock<IServiceBusBroker> serviceBusBrokerMock;
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<ILogger<ServiceBusService>> loggerMock;
    private readonly IServiceBusService serviceBusService;

    public ServiceBusServiceTests()
    {
        serviceBusBrokerMock = new Mock<IServiceBusBroker>();
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        loggerMock = new Mock<ILogger<ServiceBusService>>();
        serviceBusService = new ServiceBusService(
            serviceBusBroker: serviceBusBrokerMock.Object,
            serviceProviderBroker: serviceProviderBrokerMock.Object,
            log: loggerMock.Object);
    }
}