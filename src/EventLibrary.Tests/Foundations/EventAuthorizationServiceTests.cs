using EventLibrary.Brokers.Interfaces;
using EventLibrary.Models.Interfaces;
using EventLibrary.Services.Foundations;
using Moq;

namespace EventLibrary.Tests.Foundations;

public partial class EventAuthorizationServiceTests
{
    private readonly Mock<IEventAuthorizationBroker> eventAuthorizationBrokerMock;
    private readonly IEventAuthorizationService eventAuthorizationService;

    public EventAuthorizationServiceTests()
    {
        eventAuthorizationBrokerMock = new Mock<IEventAuthorizationBroker>();
        eventAuthorizationService = new EventAuthorizationService(eventAuthorizationBrokerMock.Object);
    }
}
