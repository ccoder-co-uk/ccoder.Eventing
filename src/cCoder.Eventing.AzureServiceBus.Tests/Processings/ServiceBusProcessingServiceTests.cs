using cCoder.Eventing.AzureServiceBus.Services.Processings;
using cCoder.Eventing.AzureServiceBus.Services.Foundations;
using cCoder.Eventing.AzureServiceBus.Models;
using Moq;

namespace cCoder.Eventing.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    private readonly Mock<IServiceBusService> serviceBusServiceMock;
    private readonly Mock<IServiceBusEventAuthInfo> eventAuthInfoMock;
    private readonly IServiceBusProcessingService serviceBusProcessingService;

    public ServiceBusProcessingServiceTests()
    {
        serviceBusServiceMock = new Mock<IServiceBusService>();
        eventAuthInfoMock = new Mock<IServiceBusEventAuthInfo>();

        serviceBusProcessingService = new ServiceBusProcessingService(
            () => eventAuthInfoMock.Object,
            serviceBusServiceMock.Object);
    }
}
