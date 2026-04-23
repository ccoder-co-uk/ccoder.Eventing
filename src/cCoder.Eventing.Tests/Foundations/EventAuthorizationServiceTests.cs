using cCoder.Eventing.Brokers;
using cCoder.Eventing.Services.Foundations;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventAuthorizationServiceTests
{
    private readonly Mock<IEventAuthorizationBroker> eventAuthorizationBrokerMock;
    private readonly Mock<ILogger<EventAuthorizationService>> loggerMock;
    private readonly IEventAuthorizationService eventAuthorizationService;

    public EventAuthorizationServiceTests()
    {
        eventAuthorizationBrokerMock = new Mock<IEventAuthorizationBroker>();
        loggerMock = new Mock<ILogger<EventAuthorizationService>>();
        eventAuthorizationService = new EventAuthorizationService(
            eventAuthorizationBrokerMock.Object,
            loggerMock.Object);
    }
}
