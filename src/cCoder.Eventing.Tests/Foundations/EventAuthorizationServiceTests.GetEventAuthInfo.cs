using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventAuthorizationServiceTests
{
    [Fact]
    public void ShouldReturnEventAuthInfo()
    {
        IEventAuthInfo expectedEventAuthInfo = Mock.Of<IEventAuthInfo>();

        eventAuthorizationBrokerMock
            .Setup(broker => broker.GetEventAuthInfo())
            .Returns(expectedEventAuthInfo);

        IEventAuthInfo actualEventAuthInfo =
            eventAuthorizationService.GetEventAuthInfo();

        actualEventAuthInfo.Should().BeSameAs(expectedEventAuthInfo);

        eventAuthorizationBrokerMock.Verify(
            broker => broker.GetEventAuthInfo(),
            Times.Once);
    }
}
