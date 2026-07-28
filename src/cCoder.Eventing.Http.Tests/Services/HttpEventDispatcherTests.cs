// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Http.Dependencies;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Http.Services.Processings;
using cCoder.Eventing.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public partial class HttpEventDispatcherTests
{
    private readonly Mock<IServiceProviderBroker> serviceProviderBrokerMock = new();
    private readonly Mock<IServiceScope> serviceScopeMock = new();
    private readonly IServiceProvider scopedServiceProvider = new ServiceCollection()
        .BuildServiceProvider();

    public HttpEventDispatcherTests()
    {
        serviceScopeMock
            .SetupGet(expression:scope => scope.ServiceProvider)
            .Returns(value:scopedServiceProvider);

        serviceProviderBrokerMock
            .Setup(expression:broker => broker.GetScopeForEvent(message:It.IsAny<EventMessage>()))
            .Returns(value:serviceScopeMock.Object);
    }

    [Fact]
    public async Task ShouldDispatchIncomingHttpMessageToRegisteredProvider()
    {
        // Given

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
eventHandlerRegistry: new HttpEventHandlerRegistry(),
eventProviders: eventProvider);

        await dispatcher.DispatchAsync(message:new HttpEventMessage
        {
            EventName = inputName,
            SSOUserId = "user-123",
            Data = "{\"value\":\"hello\"}"
        // When

        });

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected:"user-123");

        actualMessage.Data.Value.Should()
            .Be(expected:"hello");
    }

    [Fact]
    public async Task ShouldDispatchIncomingHttpMessageToRegisteredSubscription()
    {
        // Given

        string inputName = "fake-event";
        FakePayload actualPayload = null;
        IServiceProvider actualServiceProvider = null;
        HttpEventHandlerRegistry eventHandlerRegistry = new();

        eventHandlerRegistry.ListenToEvent<FakePayload>(
name: inputName,
handler: (serviceProvider, payload) =>
            {
                actualServiceProvider = serviceProvider;
                actualPayload = payload;
                return ValueTask.CompletedTask;
            });

        IHttpEventDispatcher dispatcher = CreateDispatcher(eventHandlerRegistry:eventHandlerRegistry);

        await dispatcher.DispatchAsync(message:new HttpEventMessage
        {
            EventName = inputName,
            SSOUserId = "user-123",
            Data = "{\"value\":\"hello\"}"
        // When

        });

        // Then

        actualServiceProvider.Should()
            .BeSameAs(expected:scopedServiceProvider);

        actualPayload.Should()
            .NotBeNull();

        actualPayload.Value.Should()
            .Be(expected:"hello");
    }

    private IHttpEventDispatcher CreateDispatcher(
        IHttpEventHandlerRegistry eventHandlerRegistry,
        params EventProvider[] eventProviders) =>
        new HttpEventDispatcher(
                serviceProviderBrokerMock.Object,
                eventHandlerRegistry,
                eventProviders,
                new HttpEventingOptions
                {
                    JsonSerializerOptions =
                        new JsonSerializerOptions(JsonSerializerDefaults.Web)
                },
                Mock.Of<ILogger<HttpEventDispatcher>>());
}