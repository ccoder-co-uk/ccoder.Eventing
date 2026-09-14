// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using cCoder.Eventing.Brokers.Loggings;
using Moq;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventProviderServiceTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock;
    private readonly Mock<ILoggingBroker> loggerMock;
    private readonly Mock<IServiceScope> serviceScopeMock;
    private readonly Mock<IServiceProvider> scopedServiceProviderMock;

    public EventProviderServiceTests()
    {
        serviceProviderBrokerMock = new Mock<IServiceProviderBroker>();
        loggerMock = new Mock<ILoggingBroker>();
        serviceScopeMock = new Mock<IServiceScope>();
        scopedServiceProviderMock = new Mock<IServiceProvider>();

        serviceScopeMock
            .SetupGet(expression:scope => scope.ServiceProvider)
            .Returns(value:scopedServiceProviderMock.Object);

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetScopeForEvent(eventMessage:It.IsAny<EventMessage>()))
            .Returns(value:serviceScopeMock.Object);
    }

    private IEventProviderService CreateEventProviderService(
        params EventProvider[] eventProviders)
    {
        return new EventProviderService(
            eventProviderBrokers: eventProviders.Select(
                selector: provider => new EventProviderBroker(
                    eventProvider: provider)),
            serviceProviderBroker: serviceProviderBrokerMock.Object,
            log: loggerMock.Object);
    }

    private IBulkEventProviderService CreateBulkEventProviderService(
        BulkEventProvider[] bulkEventProviders)
    {
        return new BulkEventProviderService(
            eventProviderBrokers: bulkEventProviders.Select(
                selector: provider => new BulkEventProviderBroker(
                    eventProvider: provider)),
            serviceProviderBroker: serviceProviderBrokerMock.Object,
            log: loggerMock.Object);
    }
}