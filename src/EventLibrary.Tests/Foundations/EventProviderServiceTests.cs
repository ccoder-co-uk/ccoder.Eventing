using EventLibrary.Brokers;
using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace EventLibrary.Tests.Foundations;

public partial class EventProviderServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<ILogger<EventProviderService>> loggerMock;
    private readonly Mock<IServiceScope> serviceScopeMock;
    private readonly Mock<IServiceProvider> scopedServiceProviderMock;

    public EventProviderServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        loggerMock = new Mock<ILogger<EventProviderService>>();
        serviceScopeMock = new Mock<IServiceScope>();
        scopedServiceProviderMock = new Mock<IServiceProvider>();

        serviceScopeMock
            .SetupGet(scope => scope.ServiceProvider)
            .Returns(scopedServiceProviderMock.Object);

        serviceProviderBrokerMock
            .Setup(broker => broker.GetScopeForEvent(It.IsAny<EventMessage>()))
            .Returns(serviceScopeMock.Object);
    }

    private IEventProviderService CreateEventProviderService(params EventProvider[] eventProviders) =>
        new EventProviderService(
            serviceProviderBrokerMock.Object,
            new EventingConfiguration
            {
                EventProviders = eventProviders
            },
            loggerMock.Object);

    private IEventProviderService CreateEventProviderService(
        EventProvider[] eventProviders,
        BulkEventProvider[] bulkEventProviders) =>
        new EventProviderService(
            serviceProviderBrokerMock.Object,
            new EventingConfiguration
            {
                EventProviders = eventProviders,
                BulkEventProviders = bulkEventProviders
            },
            loggerMock.Object);
}
