// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Brokers.Loggings;
using cCoder.Eventing.Services.Orchestrations;
using cCoder.Eventing.Services.Processings;
using Moq;

namespace cCoder.Eventing.Tests.Orchestrations;

public partial class EventOrchestrationServiceTests
{
    private readonly Mock<IEventProviderProcessingService> eventProviderServiceMock;
    private readonly Mock<IBulkEventProviderProcessingService> bulkEventProviderServiceMock;
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<IEventProcessingService<FakeObject>> eventProcessingServiceMock;
    private readonly EventOrchestrationService eventOrchestrationService;

    public EventOrchestrationServiceTests()
    {
        eventProviderServiceMock = new Mock<IEventProviderProcessingService>();
        bulkEventProviderServiceMock = new Mock<IBulkEventProviderProcessingService>();
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        eventProcessingServiceMock = new Mock<IEventProcessingService<FakeObject>>();

        serviceProviderBrokerMock
            .Setup(expression: broker =>
                broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value: eventProcessingServiceMock.Object);

        eventOrchestrationService = new EventOrchestrationService(
            eventProviderServiceMock.Object,
            bulkEventProviderServiceMock.Object,
            serviceProviderBrokerMock.Object,
            Mock.Of<ILoggingBroker>());
    }
}