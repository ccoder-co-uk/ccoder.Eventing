// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Models.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceTests
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

        IServiceProvider scopedServiceProvider = Mock.Of<IServiceProvider>();
        Mock<IServiceScope> serviceScopeMock = new();
        List<FakeObject> actualMessages = [];

        serviceScopeMock
            .SetupGet(expression:scope => scope.ServiceProvider)
            .Returns(value:scopedServiceProvider);

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetScopeForEvent(message:inputMessage))
            .Returns(value:serviceScopeMock.Object);

        IEnumerable<Func<IServiceProvider, FakeObject, ValueTask>> handlers =
        [
            (_, message) =>
            {
                actualMessages.Add(item:message);
                return ValueTask.CompletedTask;
            },
            (_, message) =>
            {
                actualMessages.Add(item:message);
                return ValueTask.CompletedTask;
            }
        ];

        foreach (Func<IServiceProvider, FakeObject, ValueTask> handler in handlers)
        {
            eventService.ListenToEvent(name: inputName, handler: handler);
        }

        // When

        await eventService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        actualMessages.Should()
            .HaveCount(expected:2);

        actualMessages.Should()
            .OnlyContain(predicate:message => message == inputMessage.Data);

    }

    [Fact]
    public async Task ShouldRaiseEventAsyncWhenNoHandlersExist()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        Mock<IServiceScope> serviceScopeMock = new();

        serviceScopeMock
            .SetupGet(expression:scope => scope.ServiceProvider)
            .Returns(value:Mock.Of<IServiceProvider>());

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetScopeForEvent(message:inputMessage))
            .Returns(value:serviceScopeMock.Object);

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        await raiseEventAsyncTask.Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task ShouldThrowWrappedExceptionOnRaiseEventAsyncIfBrokerFails()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        Exception innerException = new("Broker failure");
        Mock<IServiceScope> serviceScopeMock = new();

        serviceScopeMock
            .SetupGet(expression:scope => scope.ServiceProvider)
            .Returns(value:Mock.Of<IServiceProvider>());

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetScopeForEvent(message:inputMessage))
            .Returns(value:serviceScopeMock.Object);

        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetScopeForEvent(
                message: inputMessage))
            .Throws(exception:innerException);

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode:raiseEventAsyncTask);

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }

    [Fact]
    public async Task ShouldThrowWrappedExceptionOnRaiseEventAsyncIfHandlerFails()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        Exception innerException = new("Handler failure");
        Mock<IServiceScope> serviceScopeMock = new();
        IServiceProvider scopedServiceProvider = Mock.Of<IServiceProvider>();

        serviceScopeMock
            .SetupGet(expression:scope => scope.ServiceProvider)
            .Returns(value:scopedServiceProvider);

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetScopeForEvent(message:inputMessage))
            .Returns(value:serviceScopeMock.Object);

        IEnumerable<Func<IServiceProvider, FakeObject, ValueTask>> handlers =
        [
            (_, _) => throw innerException
        ];

        eventService.ListenToEvent(
            name: inputName,
            handler: handlers.Single());

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode:raiseEventAsyncTask);

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}