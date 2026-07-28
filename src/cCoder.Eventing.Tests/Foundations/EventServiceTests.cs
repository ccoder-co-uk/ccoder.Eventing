// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<ILogger<EventService<FakeObject>>> loggerMock;
    private readonly IEventService<FakeObject> eventService;

    public EventServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        loggerMock = new Mock<ILogger<EventService<FakeObject>>>();

        eventService = new EventService<FakeObject>(serviceProviderBrokerMock.Object, loggerMock.Object);
    }
}