// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    [Fact]
    public async Task ShouldListenToEventWithHandlingService()
    {
        // Given
        string inputName = "event-name";
        FakeObject inputMessage = new() { Name = "test" };
        Mock<IHandlingService> handlingServiceMock = new();
        Mock<IServiceProvider> serviceProviderMock = new();
        Func<IServiceProvider, FakeObject, ValueTask> internalHandler = null;
        IHandlingService actualHandlingService = null;
        FakeObject actualMessage = null;

        serviceProviderBrokerMock
            .Setup(expression: broker =>
                broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value: eventProcessingServiceMock.Object);

        serviceProviderBrokerMock
            .Setup(expression: broker =>
                broker.GetRequiredService<IHandlingService>(
                    serviceProvider: serviceProviderMock.Object))
            .Returns(value: handlingServiceMock.Object);

        eventProcessingServiceMock
            .Setup(expression: service => service.ListenToEvent(
                name: inputName,
                handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Callback<string, Func<IServiceProvider, FakeObject, ValueTask>>(
                action: (_, handler) => internalHandler = handler);

        eventServiceProviderService.ListenToEvent<FakeObject, IHandlingService>(
            name: inputName,
            handler: (handlingService, message) =>
            {
                actualHandlingService = handlingService;
                actualMessage = message;

                return ValueTask.CompletedTask;
            });

        // When
        await internalHandler(
            arg1: serviceProviderMock.Object,
            arg2: inputMessage);

        // Then
        actualHandlingService.Should()
            .BeSameAs(expected: handlingServiceMock.Object);

        actualMessage.Should()
            .BeSameAs(expected: inputMessage);

        serviceProviderBrokerMock.VerifyAll();
        eventProcessingServiceMock.VerifyAll();
    }

    public interface IHandlingService;
}