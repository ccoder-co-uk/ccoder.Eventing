using EventLibrary.AzureServiceBus.Brokers.Interfaces;
using EventLibrary.AzureServiceBus.Services.Foundations;
using EventLibrary.AzureServiceBus.Services.Foundations.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventLibrary.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    private readonly Mock<IServiceBusBroker> serviceBusBrokerMock;
    private readonly Mock<ILogger<ServiceBusService>> loggerMock;
    private readonly IServiceBusService serviceBusService;

    public ServiceBusServiceTests()
    {
        serviceBusBrokerMock = new Mock<IServiceBusBroker>();
        loggerMock = new Mock<ILogger<ServiceBusService>>();
        serviceBusService = new ServiceBusService(serviceBusBrokerMock.Object, loggerMock.Object);
    }
}
