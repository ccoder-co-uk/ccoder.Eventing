using EventLibrary.Brokers;
using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using EventLibrary.Services.Processings;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventLibrary.Tests.Processings;

public partial class EventProcessingServiceTests
{
    private readonly Mock<IEventService<EventMessage<FakeObject>>> eventServiceMock;
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<ILogger<EventProcessingService<FakeObject>>> loggerMock;
    private readonly IEventProcessingService<FakeObject> eventProcessingService;

    public EventProcessingServiceTests()
    {
        eventServiceMock = new Mock<IEventService<EventMessage<FakeObject>>>();
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        loggerMock = new Mock<ILogger<EventProcessingService<FakeObject>>>();

        eventProcessingService = new EventProcessingService<FakeObject>(
            eventServiceMock.Object,
            serviceProviderBrokerMock.Object,
            loggerMock.Object);
    }
}
