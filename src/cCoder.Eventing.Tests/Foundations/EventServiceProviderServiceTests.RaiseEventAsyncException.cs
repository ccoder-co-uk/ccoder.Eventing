// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Models.Exceptions;
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
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        Exception innerException = new("Processing failure");

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(name:inputName, handler:(_, _) => ValueTask.CompletedTask);

        eventProcessingServiceMock
            .Setup(expression:service => service.RaiseEventAsync(name:inputName, data:inputMessage))
            .Returns(value:new ValueTask(Task.FromException(exception:innerException)));

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode:raiseEventAsyncTask);

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }

    [Fact]
    public async Task ShouldRethrowOnRaiseEventsAsyncIfProcessingServiceFails()
    {
        // Given

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
            .Setup(expression:broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(value:eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(name:inputName, handler:(_, _) => ValueTask.CompletedTask);

        eventProcessingServiceMock
            .Setup(expression:service => service.RaiseEventAsync(name:inputName, data:inputMessages[0]))
            .Returns(value:new ValueTask(Task.FromException(exception:innerException)));

        // When

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventsAsync(name:inputName, messages:inputMessages);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode:raiseEventsAsyncTask);

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}