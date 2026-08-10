// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Processings;
using cCoder.Eventing.Brokers.Loggings;
using Moq;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<IEventProcessingService<FakeObject>> eventProcessingServiceMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly IEventServiceProviderService eventServiceProviderService;

    public EventServiceProviderServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        eventProcessingServiceMock = new Mock<IEventProcessingService<FakeObject>>();
        loggerMock = new Mock<ILoggingBroker>();

        eventServiceProviderService = new EventServiceProviderService(
            serviceProviderBrokerMock.Object,
            loggerMock.Object);
    }
}