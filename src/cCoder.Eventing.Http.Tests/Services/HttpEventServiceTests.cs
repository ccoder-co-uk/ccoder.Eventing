// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Brokers;
using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Foundations;
using cCoder.Eventing.Models;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.Services;

public class HttpEventServiceTests
{
    [Fact]
    public async Task ShouldSerializeEventPayloadForTransport()
    {
        // Given

        string inputName = "fake-event";

        EventMessage<FakePayload> inputMessage = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = "user-123" },
            Data = new FakePayload { Value = "hello" }
        };

        HttpEventMessage actualMessage = null;
        Mock<IHttpEventBroker> httpEventBrokerMock = new();

        httpEventBrokerMock
            .Setup(expression:broker => broker.SendAsync(
                It.IsAny<HttpEventMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<HttpEventMessage, CancellationToken>(
action: (message, _) => actualMessage = message)
            .Returns(value:ValueTask.CompletedTask);

        IHttpEventService httpEventService = new HttpEventService(
            httpEventBrokerMock.Object,
            new HttpEventQueue(),
            new HttpEventHandlerRegistry(),
            new HttpEventingOptions(),
            Mock.Of<ILogger<HttpEventService>>());

        // When

        await httpEventService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage.EventName.Should()
            .Be(expected:inputName);

        actualMessage.SSOUserId.Should()
            .Be(expected:inputMessage.AuthInfo.SSOUserId);

        actualMessage.Data.Should()
            .Contain(expected:"\"value\":\"hello\"");
    }
}