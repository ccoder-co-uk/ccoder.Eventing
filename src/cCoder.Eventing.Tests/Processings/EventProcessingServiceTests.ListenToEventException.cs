// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Processings;

public partial class EventProcessingServiceTests
{
    [Fact]
    public void ShouldWrapArgumentExceptionOnListenToEvent()
    {
        // Given

        ArgumentException dependencyException = new(message: "dependency failure");

        eventServiceMock
            .Setup(expression: service => service.ListenToEvent(
                name: It.IsAny<string>(),
                handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Throws(exception: dependencyException);

        // When

        Action listenAction = () => eventProcessingService.ListenToEvent(
            name: "event-name",
            handler: (_, _) => ValueTask.CompletedTask);

        // Then

        ServiceValidationException actualException =
            Assert.Throws<ServiceValidationException>(testCode: listenAction);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public void ShouldWrapInvalidOperationExceptionOnListenToEvent()
    {
        // Given

        InvalidOperationException dependencyException = new(message: "dependency failure");

        eventServiceMock
            .Setup(expression: service => service.ListenToEvent(
                name: It.IsAny<string>(),
                handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Throws(exception: dependencyException);

        // When

        Action listenAction = () => eventProcessingService.ListenToEvent(
            name: "event-name",
            handler: (_, _) => ValueTask.CompletedTask);

        // Then

        ServiceDependencyException actualException =
            Assert.Throws<ServiceDependencyException>(testCode: listenAction);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public void ShouldWrapUnexpectedExceptionOnListenToEvent()
    {
        // Given

        Exception unexpectedException = new(message: "unexpected failure");

        eventServiceMock
            .Setup(expression: service => service.ListenToEvent(
                name: It.IsAny<string>(),
                handler: It.IsAny<Func<IServiceProvider, FakeObject, ValueTask>>()))
            .Throws(exception: unexpectedException);

        // When

        Action listenAction = () => eventProcessingService.ListenToEvent(
            name: "event-name",
            handler: (_, _) => ValueTask.CompletedTask);

        // Then

        ServiceException actualException = Assert.Throws<ServiceException>(
            testCode: listenAction);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: unexpectedException);
    }
}