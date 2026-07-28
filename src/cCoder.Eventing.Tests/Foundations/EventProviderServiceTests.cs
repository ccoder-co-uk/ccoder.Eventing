// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .SetupGet(expression:scope => scope.ServiceProvider)
            .Returns(value:scopedServiceProviderMock.Object);

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetScopeForEvent(message:It.IsAny<EventMessage>()))
            .Returns(value:serviceScopeMock.Object);
    }

    private IEventProviderService CreateEventProviderService(
        params EventProvider[] eventProviders)
    {
        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetServices<EventProvider>())
            .Returns(value: eventProviders);

        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetServices<BulkEventProvider>())
            .Returns(value: []);

        return new EventProviderService(
            serviceProviderBroker: serviceProviderBrokerMock.Object,
            log: loggerMock.Object);
    }

    private IEventProviderService CreateEventProviderService(
        EventProvider[] eventProviders,
        BulkEventProvider[] bulkEventProviders)
    {
        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetServices<EventProvider>())
            .Returns(value: eventProviders);

        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetServices<BulkEventProvider>())
            .Returns(value: bulkEventProviders);

        return new EventProviderService(
            serviceProviderBroker: serviceProviderBrokerMock.Object,
            log: loggerMock.Object);
    }
}