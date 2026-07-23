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
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(name:inputName, handler:(_, _) => ValueTask.CompletedTask);

        await eventServiceProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        eventProcessingServiceMock.Verify(
expression:            service => service.RaiseEventAsync(inputName, inputMessage),
times:            Times.Once);
    }

    [Fact]
    public async Task ShouldNotThrowWhenNoHandlerIsConfigured()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        await raiseEventAsyncTask.Should().NotThrowAsync();
    }
}