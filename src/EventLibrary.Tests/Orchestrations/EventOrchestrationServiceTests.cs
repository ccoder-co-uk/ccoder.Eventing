using EventLibrary.Services.Foundations;
using EventLibrary.Services.Orchestrations;
using Moq;

namespace EventLibrary.Tests.Orchestrations;

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
