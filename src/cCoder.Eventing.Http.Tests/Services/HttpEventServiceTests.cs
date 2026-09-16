// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Brokers;
using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Models.Exceptions;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Text.Json;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public sealed partial class HttpEventServiceTests
{
    [Fact]
    public async Task ShouldDispatchHttpEventMessageToSubscription()
    {
        // Given

        FakePayload actualPayload = null;
        IServiceProvider actualServiceProvider = null;

        IServiceProvider scopedServiceProvider = new ServiceCollection()
            .BuildServiceProvider();

        HttpEventService service = CreateService(
            scopedServiceProvider: scopedServiceProvider);

        service.ListenToEvent<FakePayload>(
            name: "event",
            handler: (serviceProvider, payload) =>
            {
                actualServiceProvider = serviceProvider;
                actualPayload = payload;
                return ValueTask.CompletedTask;
            });

        // When

        bool handled = await service.DispatchHttpEventMessageAsync(
            httpEventMessage: new HttpEventMessage
            {
                EventName = "event",
                SSOUserId = "user-123",
                Data = "{\"value\":\"hello\"}"
            });

        // Then

        handled
            .Should()
            .BeTrue();

        actualServiceProvider
            .Should()
            .BeSameAs(expected: scopedServiceProvider);

        actualPayload.Value
            .Should()
            .Be(expected: "hello");
    }

    [Fact]
    public async Task ShouldDispatchHttpEventMessageToProvider()
    {
        // Given

        EventMessage<FakePayload> actualMessage = null;

        HttpEventProviderBrokerConfiguration providerConfiguration = new()
        {
            CanReceive = name => name == "event",
            DataType = typeof(FakePayload),
            ReceiveAsync = (_, _, message) =>
            {
                actualMessage = (EventMessage<FakePayload>)message;
                return ValueTask.CompletedTask;
            },
            JsonSerializerOptions =
                new JsonSerializerOptions(JsonSerializerDefaults.Web)
        };

        HttpEventService service = CreateService(
            eventProviderConfigurations: [providerConfiguration]);

        // When

        bool handled = await service.DispatchHttpEventMessageAsync(
            httpEventMessage: new HttpEventMessage
            {
                EventName = "event",
                SSOUserId = "user-123",
                Data = "{\"value\":\"hello\"}"
            });

        // Then

        handled
            .Should()
            .BeTrue();

        actualMessage.AuthInfo.SSOUserId
            .Should()
            .Be(expected: "user-123");

        actualMessage.Data.Value
            .Should()
            .Be(expected: "hello");
    }

    [Fact]
    public async Task ShouldRejectInvalidIncomingHttpEventMessage()
    {
        // Given

        HttpEventService service = CreateService();

        Func<Task> enqueue = async () =>
            await service.EnqueueHttpEventMessageAsync(
                httpEventMessage: new HttpEventMessage());

        // When

        Func<Task> enqueueInvalidHttpEventMessage = enqueue;

        // Then

        await enqueueInvalidHttpEventMessage
            .Should()
            .ThrowAsync<ServiceValidationException>();
    }

    private static HttpEventService CreateService(
        IServiceProvider scopedServiceProvider = null,
        IReadOnlyCollection<HttpEventProviderBrokerConfiguration>
            eventProviderConfigurations = null)
    {
        JsonSerializerOptions jsonSerializerOptions =
            new(JsonSerializerDefaults.Web);

        HttpEventBroker broker = new(
            httpClientFactory: Mock.Of<IHttpClientFactory>(),
            configuration: new HttpEventBrokerConfiguration
            {
                MaxConcurrency = 1,
                JsonSerializerOptions = jsonSerializerOptions,
                EventProviderConfigurations =
                    eventProviderConfigurations ?? []
            });

        Mock<IServiceScope> scope = new();

        scope.SetupGet(expression: value => value.ServiceProvider)
            .Returns(value: scopedServiceProvider
                ?? new ServiceCollection().BuildServiceProvider());

        Mock<IServiceProviderBroker> serviceProviderBroker = new();

        serviceProviderBroker
            .Setup(expression: value => value.GetScopeForEvent(
                eventMessage: It.IsAny<EventMessage>()))
            .Returns(value: scope.Object);

        return new HttpEventService(
            httpEventBroker: broker,
            serviceProviderBroker: serviceProviderBroker.Object);
    }
}