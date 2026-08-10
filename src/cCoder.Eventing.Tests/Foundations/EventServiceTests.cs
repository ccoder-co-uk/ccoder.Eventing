// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;
using cCoder.Eventing.Brokers.Loggings;
using Moq;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly IEventService<FakeObject> eventService;

    public EventServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        loggerMock = new Mock<ILoggingBroker>();

        eventService = new EventService<FakeObject>(serviceProviderBrokerMock.Object, loggerMock.Object);
    }
}