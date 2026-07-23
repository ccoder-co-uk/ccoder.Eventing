// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using cCoder.Eventing.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public class HttpEventDispatcherTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock = new();
    private readonly Mock<IServiceScope> serviceScopeMock = new();
    private readonly IServiceProvider scopedServiceProvider = new ServiceCollection().BuildServiceProvider();

    public HttpEventDispatcherTests()
    {
        serviceScopeMock
            .SetupGet(scope => scope.ServiceProvider)
            .Returns(scopedServiceProvider);

        serviceProviderBrokerMock
            .Setup(broker => broker.GetScopeForEvent(It.IsAny<EventMessage>()))
            .Returns(serviceScopeMock.Object);
    }

    [Fact]
    public async Task ShouldDispatchIncomingHttpMessageToRegisteredProvider()
    {
        string inputName = "fake-event";
        EventMessage<FakePayload> actualMessage = null;

        EventProvider<FakePayload> eventProvider = new()
        {
            Events = [inputName],
            ReceiveHandler = (_, _, message) =>
            {
                actualMessage = message;
                return ValueTask.CompletedTask;
            }
        };

        IHttpEventDispatcher dispatcher = CreateDispatcher(
            new HttpEventHandlerRegistry(),
            eventProvider);

        await dispatcher.DispatchAsync(new HttpEventMessage
        {
            EventName = inputName,
            SSOUserId = "user-123",
            Data = "{\"value\":\"hello\"}"
        });

        actualMessage.Should().NotBeNull();
        actualMessage.AuthInfo.SSOUserId.Should().Be("user-123");
        actualMessage.Data.Value.Should().Be("hello");
    }

    [Fact]
    public async Task ShouldDispatchIncomingHttpMessageToRegisteredSubscription()
    {
        string inputName = "fake-event";
        FakePayload actualPayload = null;
        IServiceProvider actualServiceProvider = null;
        HttpEventHandlerRegistry eventHandlerRegistry = new();

        eventHandlerRegistry.ListenToEvent<FakePayload>(
            inputName,
            (serviceProvider, payload) =>
            {
                actualServiceProvider = serviceProvider;
                actualPayload = payload;
                return ValueTask.CompletedTask;
            });

        IHttpEventDispatcher dispatcher = CreateDispatcher(eventHandlerRegistry);

        await dispatcher.DispatchAsync(new HttpEventMessage
        {
            EventName = inputName,
            SSOUserId = "user-123",
            Data = "{\"value\":\"hello\"}"
        });

        actualServiceProvider.Should().BeSameAs(scopedServiceProvider);
        actualPayload.Should().NotBeNull();
        actualPayload.Value.Should().Be("hello");
    }

    private IHttpEventDispatcher CreateDispatcher(
        IHttpEventHandlerRegistry eventHandlerRegistry,
        params EventProvider[] eventProviders) =>
            new HttpEventDispatcher(
                serviceProviderBrokerMock.Object,
                eventHandlerRegistry,
                eventProviders,
                new HttpEventingOptions(),
                Mock.Of<ILogger<HttpEventDispatcher>>());
}