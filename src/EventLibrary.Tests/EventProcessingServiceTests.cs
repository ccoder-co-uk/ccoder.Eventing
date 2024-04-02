using EventLibrary.Brokers.Interfaces;
using EventLibrary.Services.Processing;
using EventLibrary.Services.Processing.Interfaces;
using EventLibrary.Tests.TestServices;
using Moq;

namespace EventLibrary.Tests
{
    public class EventProcessingServiceTests
    {
        readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;

        readonly IEventProcessingService<FakeObject> eventProcessingService;

        public EventProcessingServiceTests()
        {
            serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();

            eventProcessingService = 
                new EventProcessingService<FakeObject>(serviceProviderBrokerMock.Object);
        }
    }
}