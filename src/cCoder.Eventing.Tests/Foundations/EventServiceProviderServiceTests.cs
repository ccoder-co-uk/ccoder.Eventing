// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Processings;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<IEventProcessingService<FakeObject>> eventProcessingServiceMock;
    private readonly Mock<ILogger<EventServiceProviderService>> loggerMock;
    private readonly IEventServiceProviderService eventServiceProviderService;

    public EventServiceProviderServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        eventProcessingServiceMock = new Mock<IEventProcessingService<FakeObject>>();
        loggerMock = new Mock<ILogger<EventServiceProviderService>>();

        eventServiceProviderService = new EventServiceProviderService(
            serviceProviderBrokerMock.Object,
            loggerMock.Object);
    }
}