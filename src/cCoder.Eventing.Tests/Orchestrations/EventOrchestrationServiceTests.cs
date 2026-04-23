using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Orchestrations;
using Moq;

namespace cCoder.Eventing.Tests.Orchestrations;

public partial class EventOrchestrationServiceTests
{
    private readonly Mock<IEventProviderService> eventProviderServiceMock;
    private readonly Mock<IEventServiceProviderService> eventServiceProviderServiceMock;
    private readonly EventOrchestrationService eventOrchestrationService;

    public EventOrchestrationServiceTests()
    {
        eventProviderServiceMock = new Mock<IEventProviderService>();
        eventServiceProviderServiceMock = new Mock<IEventServiceProviderService>();

        eventOrchestrationService = new EventOrchestrationService(
            eventProviderServiceMock.Object,
            eventServiceProviderServiceMock.Object);
    }
}
