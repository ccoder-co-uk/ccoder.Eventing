// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsync()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(name:inputName, handler:(_, _) => ValueTask.CompletedTask);

        // When

        await eventServiceProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        eventProcessingServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, data:inputMessage),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldNotThrowWhenNoHandlerIsConfigured()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        await raiseEventAsyncTask.Should()
            .NotThrowAsync();
    }
}