// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Brokers.Loggings;
using Moq;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventAuthorizationServiceTests
{
    private readonly Mock<IEventAuthorizationBroker> eventAuthorizationBrokerMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly IEventAuthorizationService eventAuthorizationService;

    public EventAuthorizationServiceTests()
    {
        eventAuthorizationBrokerMock = new Mock<IEventAuthorizationBroker>();
        loggerMock = new Mock<ILoggingBroker>();
        eventAuthorizationService = new EventAuthorizationService(
            eventAuthorizationBrokerMock.Object,
            loggerMock.Object);
    }
}