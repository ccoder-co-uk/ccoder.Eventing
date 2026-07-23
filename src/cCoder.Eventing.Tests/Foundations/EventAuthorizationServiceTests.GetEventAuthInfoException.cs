// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventAuthorizationServiceTests
{
    [Fact]
    public void ShouldThrowWrappedExceptionOnGetEventAuthInfoIfBrokerFails()
    {
        Exception innerException = new("Broker failure");

        eventAuthorizationBrokerMock
            .Setup(broker => broker.GetEventAuthInfo())
            .Throws(innerException);

        Action getEventAuthInfoAction = () => eventAuthorizationService.GetEventAuthInfo();

        Exception actualException =
            getEventAuthInfoAction.Should().Throw<Exception>().Which;

        actualException.Should().BeSameAs(innerException);
    }
}