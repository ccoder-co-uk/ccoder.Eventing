using EventLibrary.Brokers.Interfaces;
using EventLibrary.Services.Foundation;
using EventLibrary.Services.Foundation.Interfaces;
using EventLibrary.Tests.TestServices;
using Moq;

namespace EventLibrary.Tests
{
    public partial class EventServiceTests
    {
        readonly Mock<IEventBroker<FakeObject>> brokerMock;
        readonly Mock<ILogger<EventService<FakeObject>>> logMock;
        readonly IEventService<FakeObject> eventService;

        public EventServiceTests()
        {
            brokerMock = new Mock<IEventBroker<FakeObject>>();
            logMock = new Mock<ILogger<EventService<FakeObject>>>();
            eventService = new EventService<FakeObject>(brokerMock.Object, logMock.Object);
        }
    }
}