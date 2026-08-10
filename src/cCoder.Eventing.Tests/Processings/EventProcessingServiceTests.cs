// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Processings;
using cCoder.Eventing.Brokers.Loggings;
using Moq;

namespace cCoder.Eventing.Tests.Processings;

public partial class EventProcessingServiceTests
{
    private readonly Mock<IEventService<FakeObject>> eventServiceMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly IEventProcessingService<FakeObject> eventProcessingService;

    public EventProcessingServiceTests()
    {
        eventServiceMock = new Mock<IEventService<FakeObject>>();
        loggerMock = new Mock<ILoggingBroker>();

        eventProcessingService = new EventProcessingService<FakeObject>(
            eventServiceMock.Object,
            loggerMock.Object);
    }
}