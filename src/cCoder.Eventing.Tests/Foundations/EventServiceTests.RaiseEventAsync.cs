// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
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
            .SetupGet(scope => scope.ServiceProvider)
            .Returns(value:scopedServiceProvider);

        serviceProviderBrokerMock
            .Setup(broker => broker.GetScopeForEvent(inputMessage))
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

        eventBrokerMock
            .Setup(broker => broker.GetHandlers(inputName))
            .Returns(value:handlers);

        await eventService.RaiseEventAsync(name:inputName, message:inputMessage);

        actualMessages.Should().HaveCount(expected:2);
        actualMessages.Should().OnlyContain(predicate:message => message == inputMessage.Data);

        eventBrokerMock.Verify(
expression:            broker => broker.GetHandlers(inputName),
times:            Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncWhenNoHandlersExist()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };
        Mock<IServiceScope> serviceScopeMock = new();

        serviceScopeMock
            .SetupGet(scope => scope.ServiceProvider)
            .Returns(value:Mock.Of<IServiceProvider>());

        serviceProviderBrokerMock
            .Setup(broker => broker.GetScopeForEvent(inputMessage))
            .Returns(value:serviceScopeMock.Object);

        eventBrokerMock
            .Setup(broker => broker.GetHandlers(inputName))
            .Returns(value:Array.Empty<Func<IServiceProvider, FakeObject, ValueTask>>());

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(name:inputName, message:inputMessage);

        await raiseEventAsyncTask.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ShouldThrowWrappedExceptionOnRaiseEventAsyncIfBrokerFails()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };
        Exception innerException = new("Broker failure");
        Mock<IServiceScope> serviceScopeMock = new();

        serviceScopeMock
            .SetupGet(scope => scope.ServiceProvider)
            .Returns(value:Mock.Of<IServiceProvider>());

        serviceProviderBrokerMock
            .Setup(broker => broker.GetScopeForEvent(inputMessage))
            .Returns(value:serviceScopeMock.Object);

        eventBrokerMock
            .Setup(broker => broker.GetHandlers(inputName))
            .Throws(exception:innerException);

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(name:inputName, message:inputMessage);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(testCode:raiseEventAsyncTask);

        actualException.Should().BeSameAs(expected:innerException);
    }

    [Fact]
    public async Task ShouldThrowWrappedExceptionOnRaiseEventAsyncIfHandlerFails()
    {
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
            .SetupGet(scope => scope.ServiceProvider)
            .Returns(value:scopedServiceProvider);

        serviceProviderBrokerMock
            .Setup(broker => broker.GetScopeForEvent(inputMessage))
            .Returns(value:serviceScopeMock.Object);

        IEnumerable<Func<IServiceProvider, FakeObject, ValueTask>> handlers =
        [
            (_, _) => throw innerException
        ];

        eventBrokerMock
            .Setup(broker => broker.GetHandlers(inputName))
            .Returns(value:handlers);

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(name:inputName, message:inputMessage);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(testCode:raiseEventAsyncTask);

        actualException.Should().BeSameAs(expected:innerException);
    }
}