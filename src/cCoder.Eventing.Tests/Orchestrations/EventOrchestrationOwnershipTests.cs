// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Orchestrations;
using cCoder.Eventing.Services.Processings;
using cCoder.Eventing.Brokers.Loggings;
using Moq;

namespace cCoder.Eventing.Tests.Orchestrations;

public partial class EventOrchestrationOwnershipTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<IEventProcessingService<FakeObject>> eventProcessingServiceMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly EventOrchestrationService eventServiceProviderService;

    public EventOrchestrationOwnershipTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        eventProcessingServiceMock = new Mock<IEventProcessingService<FakeObject>>();
        loggerMock = new Mock<ILoggingBroker>();

        eventServiceProviderService = new EventOrchestrationService(
            Mock.Of<IEventProviderProcessingService>(),
            Mock.Of<IBulkEventProviderProcessingService>(),
            serviceProviderBrokerMock.Object,
            loggerMock.Object);
    }
}