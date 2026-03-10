using EventLibrary.Brokers;
using EventLibrary.Services.Foundations;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventLibrary.Tests.Foundations;

public partial class EventServiceTests
{
    private readonly Mock<IEventBroker<FakeObject>> eventBrokerMock;
    private readonly Mock<ILogger<EventService<FakeObject>>> loggerMock;
    private readonly IEventService<FakeObject> eventService;

    public EventServiceTests()
    {
        eventBrokerMock = new Mock<IEventBroker<FakeObject>>();
        loggerMock = new Mock<ILogger<EventService<FakeObject>>>();
        eventService = new EventService<FakeObject>(eventBrokerMock.Object, loggerMock.Object);
    }
}
