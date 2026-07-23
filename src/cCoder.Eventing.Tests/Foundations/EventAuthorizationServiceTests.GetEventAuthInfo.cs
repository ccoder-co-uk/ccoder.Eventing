// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

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
            .Returns(value:expectedEventAuthInfo);

        IEventAuthInfo actualEventAuthInfo =
            eventAuthorizationService.GetEventAuthInfo();

        actualEventAuthInfo.Should().BeSameAs(expected:expectedEventAuthInfo);

        eventAuthorizationBrokerMock.Verify(
expression:            broker => broker.GetEventAuthInfo(),
times:            Times.Once);
    }
}