using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Processings;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Eventing.Tests.Processings;

public partial class EventProcessingServiceTests
{
    private readonly Mock<IEventService<FakeObject>> eventServiceMock;
    private readonly Mock<ILogger<EventProcessingService<FakeObject>>> loggerMock;
    private readonly IEventProcessingService<FakeObject> eventProcessingService;

    public EventProcessingServiceTests()
    {
        eventServiceMock = new Mock<IEventService<FakeObject>>();
        loggerMock = new Mock<ILogger<EventProcessingService<FakeObject>>>();

        eventProcessingService = new EventProcessingService<FakeObject>(
            eventServiceMock.Object,
            loggerMock.Object);
    }
}
