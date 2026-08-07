// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Dependencies;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Transports;

public partial class HttpTransportDependencyTests
{
    [Fact]
    public async Task ShouldPostMessagesToConfiguredHub()
    {
        // Given

        HttpClient httpClient = new(handler: new SuccessfulHandler());
        Mock<IHttpClientFactory> clientFactory = new();

        clientFactory
            .Setup(expression: factory => factory.CreateClient(
                name: HttpEventingOptions.HttpClientName))
            .Returns(value: httpClient);

        HttpEventBroker broker = new(
            httpClientFactory: clientFactory.Object,
            options: new HttpEventingOptions
            {
                HubUrl = "https://example.test/events",
                JsonSerializerOptions =
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)
            },
            log: NullLogger<HttpEventBroker>.Instance);

        // When

        await broker.SendAsync(message: new HttpEventMessage());

        // Then

        clientFactory.Verify(
            expression: factory => factory.CreateClient(
                name: HttpEventingOptions.HttpClientName),
            times: Times.Once);
    }

    [Fact]
    public async Task ShouldRejectMissingHubConfiguration()
    {
        // Given

        HttpEventBroker broker = new(
            httpClientFactory: Mock.Of<IHttpClientFactory>(),
            options: new HttpEventingOptions(),
            log: NullLogger<HttpEventBroker>.Instance);

        // When

        Func<Task> send = async () => await broker.SendAsync(
            message: new HttpEventMessage());

        // Then

        await send
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public void ShouldRegisterValidateAndFilterSubscriptions()
    {
        // Given

        const string eventName = "event";
        HttpEventHandlerRegistry registry = new();

        Func<IServiceProvider, FakePayload, ValueTask> handler =
            (_, _) => ValueTask.CompletedTask;

        // When

        registry.ListenToEvent(name: eventName, handler: handler);

        IReadOnlyCollection<HttpEventSubscription> matching =
            registry.GetSubscriptions(name: eventName);

        IReadOnlyCollection<HttpEventSubscription> missing =
            registry.GetSubscriptions(name: "missing");

        Action missingName = () => registry.ListenToEvent(
            name: null,
            handler: handler);

        Action missingHandler = () => registry.ListenToEvent<FakePayload>(
            name: eventName,
            handler: null);

        // Then

        matching
            .Should()
            .ContainSingle();

        missing
            .Should()
            .BeEmpty();

        missingName
            .Should()
            .Throw<InvalidOperationException>();

        missingHandler
            .Should()
            .Throw<InvalidOperationException>();
    }

    private sealed class SuccessfulHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(
                result: new HttpResponseMessage(HttpStatusCode.Accepted));
    }
}