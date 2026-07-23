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
    public async Task ShouldRethrowOnRaiseEventAsyncIfProcessingServiceFails()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };
        Exception innerException = new("Processing failure");

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(name:inputName, handler:(_, _) => ValueTask.CompletedTask);

        eventProcessingServiceMock
            .Setup(service => service.RaiseEventAsync(inputName, inputMessage))
            .Returns(value:new ValueTask(Task.FromException(innerException)));

        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(testCode:raiseEventAsyncTask);

        actualException.Should().BeSameAs(expected:innerException);
    }

    [Fact]
    public async Task ShouldRethrowOnRaiseEventsAsyncIfProcessingServiceFails()
    {
        string inputName = "event-name";
        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];
        Exception innerException = new("Processing failure");

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(name:inputName, handler:(_, _) => ValueTask.CompletedTask);

        eventProcessingServiceMock
            .Setup(service => service.RaiseEventAsync(inputName, inputMessages[0]))
            .Returns(value:new ValueTask(Task.FromException(innerException)));

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(testCode:raiseEventsAsyncTask);

        actualException.Should().BeSameAs(expected:innerException);
    }
}