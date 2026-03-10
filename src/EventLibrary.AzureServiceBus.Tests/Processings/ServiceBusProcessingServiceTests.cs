using EventLibrary.AzureServiceBus.Services.Foundations.Interfaces;
using EventLibrary.AzureServiceBus.Services.Processings;
using EventLibrary.AzureServiceBus.Services.Processings.Interfaces;
using EventLibrary.Models.Interfaces;
using Moq;

namespace EventLibrary.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    private readonly Mock<IServiceBusService> serviceBusServiceMock;
    private readonly Mock<IEventAuthInfo> eventAuthInfoMock;
    private readonly IServiceBusProcessingService serviceBusProcessingService;

    public ServiceBusProcessingServiceTests()
    {
        serviceBusServiceMock = new Mock<IServiceBusService>();
        eventAuthInfoMock = new Mock<IEventAuthInfo>();
        serviceBusProcessingService = new ServiceBusProcessingService(
            () => eventAuthInfoMock.Object,
            serviceBusServiceMock.Object);
    }
}
