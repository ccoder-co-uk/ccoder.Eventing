// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Foundations;

public partial class ServiceBusServiceTests
{
    [Fact]
    public async Task ShouldHandleReceivedEventThroughScopedServiceProvider()
    {
        // Given

        const string eventName = "event-name";
        FakeObject expectedData = new() { Name = "test" };
        Mock<IServiceScope> serviceScopeMock = new();
        Mock<IServiceProvider> scopedServiceProviderMock = new();
        IServiceProvider actualServiceProvider = null;
        FakeObject actualData = null;
        Func<ServiceBusEventMessage<FakeObject>, ValueTask> messageHandler = null;

        serviceScopeMock
            .SetupGet(expression: scope => scope.ServiceProvider)
            .Returns(value: scopedServiceProviderMock.Object);

        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetScopeForEvent(
                message: It.IsAny<ServiceBusEventMessage<FakeObject>>()))
            .Returns(value: serviceScopeMock.Object);

        serviceBusBrokerMock
            .Setup(expression: broker => broker.Listen<FakeObject>(
                name: eventName,
                handler: It.IsAny<Func<ServiceBusEventMessage<FakeObject>, ValueTask>>(),
                errorHandler: It.IsAny<Func<Exception, Task>>()))
            .Callback<string,
                Func<ServiceBusEventMessage<FakeObject>, ValueTask>,
                Func<Exception, Task>>(action: (_, handler, _) =>
                    messageHandler = handler);

        ServiceBusEventMessage<FakeObject> message = new() { Data = expectedData };

        ServiceBusService.ListenToEvent<FakeObject>(
            name: eventName,
            handler: (serviceProvider, data) =>
            {
                actualServiceProvider = serviceProvider;
                actualData = data;

                return ValueTask.CompletedTask;
            });

        // When

        await messageHandler(arg: message);

        // Then

        actualServiceProvider
            .Should()
            .BeSameAs(expected: scopedServiceProviderMock.Object);

        actualData
            .Should()
            .BeSameAs(expected: expectedData);

        serviceScopeMock.Verify(
            expression: scope => scope.Dispose(),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldLogAndRethrowReceivedEventHandlerFailure()
    {
        // Given

        const string eventName = "event-name";
        Exception expectedException = new(message: "handler failure");
        Mock<IServiceScope> serviceScopeMock = new();
        Mock<IServiceProvider> scopedServiceProviderMock = new();
        Func<ServiceBusEventMessage<FakeObject>, ValueTask> messageHandler = null;

        serviceScopeMock.SetupGet(expression: scope => scope.ServiceProvider)
            .Returns(value: scopedServiceProviderMock.Object);

        serviceProviderBrokerMock
            .Setup(expression: broker => broker.GetScopeForEvent(
                message: It.IsAny<ServiceBusEventMessage<FakeObject>>()))
            .Returns(value: serviceScopeMock.Object);

        serviceBusBrokerMock
            .Setup(expression: broker => broker.Listen<FakeObject>(
                name: eventName,
                handler: It.IsAny<Func<ServiceBusEventMessage<FakeObject>, ValueTask>>(),
                errorHandler: It.IsAny<Func<Exception, Task>>()))
            .Callback<string,
                Func<ServiceBusEventMessage<FakeObject>, ValueTask>,
                Func<Exception, Task>>(action: (_, handler, _) =>
                    messageHandler = handler);

        ServiceBusService.ListenToEvent<FakeObject>(
            name: eventName,
            handler: (_, _) => ValueTask.FromException(exception: expectedException));

        ServiceBusEventMessage<FakeObject> message = new() { Data = new FakeObject() };

        // When

        Func<Task> handlingTask = async () => await messageHandler(arg: message);

        // Then

        Exception actualException = await Assert.ThrowsAsync<Exception>(testCode: handlingTask);

        actualException
            .Should()
            .BeSameAs(expected: expectedException);

        loggerMock.Verify(
            expression: logger => logger.LogError(
                exception: expectedException,
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldLogServiceBusListenerFailure()
    {
        // Given

        const string eventName = "event-name";
        Exception expectedException = new(message: "listener failure");
        Func<Exception, Task> errorHandler = null;

        serviceBusBrokerMock
            .Setup(expression: broker => broker.Listen<FakeObject>(
                name: eventName,
                handler: It.IsAny<Func<ServiceBusEventMessage<FakeObject>, ValueTask>>(),
                errorHandler: It.IsAny<Func<Exception, Task>>()))
            .Callback<string,
                Func<ServiceBusEventMessage<FakeObject>, ValueTask>,
                Func<Exception, Task>>(action: (_, _, handler) =>
                    errorHandler = handler);

        ServiceBusService.ListenToEvent<FakeObject>(
            name: eventName,
            handler: (_, _) => ValueTask.CompletedTask);

        // When

        await errorHandler(arg: expectedException);

        // Then

        loggerMock.Verify(
            expression: logger => logger.LogError(
                exception: expectedException,
                message: It.IsAny<string>(),
                args: It.IsAny<object[]>()),
            times: Times.Once);
    }
}