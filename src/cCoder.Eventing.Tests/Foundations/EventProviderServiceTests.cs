using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace cCoder.Eventing.Tests.Foundations;

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
            eventProviders,
            [],
            loggerMock.Object);

    private IEventProviderService CreateEventProviderService(
        EventProvider[] eventProviders,
        BulkEventProvider[] bulkEventProviders) =>
        new EventProviderService(
            serviceProviderBrokerMock.Object,
            eventProviders,
            bulkEventProviders,
            loggerMock.Object);
}
