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
        string inputName = "fake-event";
        EventMessage<FakePayload> inputMessage = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = "user-123" },
            Data = new FakePayload { Value = "hello" }
        };

        HttpEventMessage actualMessage = null;
        Mock<IHttpEventBroker> httpEventBrokerMock = new();

        httpEventBrokerMock
            .Setup(broker => broker.SendAsync(
                It.IsAny<HttpEventMessage>(),
                It.IsAny<CancellationToken>()))
            .Callback<HttpEventMessage, CancellationToken>(
                (message, _) => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        IHttpEventService httpEventService = new HttpEventService(
            httpEventBrokerMock.Object,
            new HttpEventQueue(),
            new HttpEventHandlerRegistry(),
            new HttpEventingOptions(),
            Mock.Of<ILogger<HttpEventService>>());

        await httpEventService.RaiseEventAsync(inputName, inputMessage);

        actualMessage.Should().NotBeNull();
        actualMessage.EventName.Should().Be(inputName);
        actualMessage.SSOUserId.Should().Be(inputMessage.AuthInfo.SSOUserId);
        actualMessage.Data.Should().Contain("\"value\":\"hello\"");
    }
}
