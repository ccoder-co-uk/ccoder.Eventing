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
        // Given

        Exception innerException = new("Broker failure");

        eventAuthorizationBrokerMock
            .Setup(expression:broker => broker.GetEventAuthInfo())
            .Throws(exception:innerException);

        // When

        Action getEventAuthInfoAction = () => eventAuthorizationService.GetEventAuthInfo();

        // Then

        Exception actualException =
            getEventAuthInfoAction.Should()
                .Throw<Exception>()
                .Which;

        actualException.Should()
            .BeSameAs(expected:innerException);
    }
}