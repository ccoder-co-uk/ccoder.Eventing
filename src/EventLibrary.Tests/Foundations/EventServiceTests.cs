using EventLibrary.Brokers;
using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventLibrary.Tests.Foundations;

public partial class EventServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<IEventBroker<FakeObject>> eventBrokerMock;
    private readonly Mock<ILogger<EventService<FakeObject>>> loggerMock;
    private readonly IEventService<FakeObject> eventService;

    public EventServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        eventBrokerMock = new Mock<IEventBroker<FakeObject>>();
        loggerMock = new Mock<ILogger<EventService<FakeObject>>>();

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventBroker<FakeObject>>())
            .Returns(eventBrokerMock.Object);

        eventService = new EventService<FakeObject>(serviceProviderBrokerMock.Object, loggerMock.Object);
    }
}
