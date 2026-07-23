// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models.Exceptions;
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

        ServiceException actualException =
            getEventAuthInfoAction.Should()
                .Throw<ServiceException>()
                .Which;

        actualException.InnerException.Should()
            .BeSameAs(expected:innerException);
    }
}