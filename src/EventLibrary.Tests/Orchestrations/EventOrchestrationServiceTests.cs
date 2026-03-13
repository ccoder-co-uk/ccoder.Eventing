using EventLibrary.Services.Foundations;
using EventLibrary.Services.Orchestrations;
using Moq;

namespace EventLibrary.Tests.Orchestrations;

public partial class EventOrchestrationServiceTests
{
    private readonly Mock<IEventServiceProviderService> eventServiceProviderServiceMock;
    private readonly EventOrchestrationService eventOrchestrationService;

    public EventOrchestrationServiceTests()
    {
        eventServiceProviderServiceMock = new Mock<IEventServiceProviderService>();

        eventOrchestrationService = new EventOrchestrationService(
            eventServiceProviderServiceMock.Object);
    }
}
