// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using FluentAssertions;
using Moq;
using System.Net;
using System.Text.Json;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Brokers;

public sealed partial class HttpEventBrokerTests
{
    [Fact]
    public async Task ShouldPostHttpEventMessageToConfiguredHub()
    {
        // Given

        HttpClient httpClient = new(handler: new SuccessfulHandler());
        Mock<IHttpClientFactory> clientFactory = new();

        clientFactory
            .Setup(expression: factory => factory.CreateClient(
                name: HttpEventingOptions.HttpClientName))
            .Returns(value: httpClient);

        HttpEventBroker broker = CreateBroker(
            httpClientFactory: clientFactory.Object,
            hubUrl: "https://example.test/events");

        // When

        await broker.SendAsync(httpEventMessage: new HttpEventMessage());

        // Then

        clientFactory.Verify(
            expression: factory => factory.CreateClient(
                name: HttpEventingOptions.HttpClientName),
            times: Times.Once);
    }

    [Fact]
    public void ShouldRegisterAndFilterHttpEventSubscriptions()
    {
        // Given

        const string eventName = "event";
        HttpEventBroker broker = CreateBroker();

        // When

        broker.InsertSubscription<FakePayload>(
            name: eventName,
            handler: (_, _) => ValueTask.CompletedTask);

        IReadOnlyCollection<HttpEventSubscription> matchingSubscriptions =
            broker.SelectSubscriptions(name: eventName);

        IReadOnlyCollection<HttpEventSubscription> missingSubscriptions =
            broker.SelectSubscriptions(name: "missing");

        // Then

        matchingSubscriptions
            .Should()
            .ContainSingle();

        missingSubscriptions
            .Should()
            .BeEmpty();
    }

    [Fact]
    public async Task ShouldQueueAndReadHttpEventMessage()
    {
        // Given

        HttpEventBroker broker = CreateBroker();

        HttpEventMessage expected = new()
        {
            EventName = "event",
            Data = "{}"
        };

        // When

        await broker.EnqueueAsync(httpEventMessage: expected);

        HttpEventMessage actual = null;

        await foreach (HttpEventMessage queuedHttpEventMessage in
            broker.ReadAllAsync(cancellationToken: default))
        {
            actual = queuedHttpEventMessage;
            break;
        }

        // Then

        actual
            .Should()
            .BeSameAs(expected: expected);
    }

    private static HttpEventBroker CreateBroker(
        IHttpClientFactory httpClientFactory = null,
        string hubUrl = null) =>
        new(
            httpClientFactory: httpClientFactory
                ?? Mock.Of<IHttpClientFactory>(),
            configuration: new HttpEventBrokerConfiguration
            {
                HubUrl = hubUrl,
                MaxConcurrency = 1,
                JsonSerializerOptions =
                    new JsonSerializerOptions(JsonSerializerDefaults.Web),
                EventProviderConfigurations = []
            });

    private sealed class SuccessfulHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken) =>
            Task.FromResult(
                result: new HttpResponseMessage(HttpStatusCode.Accepted));
    }
}