// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Processings;

public partial class EventProcessingServiceTests
{
    [Fact]
    public async Task ShouldWrapArgumentExceptionOnRaiseEventAsync()
    {
        // Given

        ArgumentException dependencyException = new(message: "dependency failure");
        EventMessage<FakeObject> message = new() { Data = new FakeObject() };

        eventServiceMock
            .Setup(expression: service => service.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: message))
            .Returns(value: ValueTask.FromException(exception: dependencyException));

        // When

        Func<Task> raiseTask = async () => await eventProcessingService.RaiseEventAsync(
            name: "event-name",
            data: message);

        // Then

        ServiceValidationException actualException =
            await Assert.ThrowsAsync<ServiceValidationException>(testCode: raiseTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public async Task ShouldWrapInvalidOperationExceptionOnRaiseEventAsync()
    {
        // Given

        InvalidOperationException dependencyException = new(message: "dependency failure");
        EventMessage<FakeObject> message = new() { Data = new FakeObject() };

        eventServiceMock
            .Setup(expression: service => service.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: message))
            .Returns(value: ValueTask.FromException(exception: dependencyException));

        // When

        Func<Task> raiseTask = async () => await eventProcessingService.RaiseEventAsync(
            name: "event-name",
            data: message);

        // Then

        ServiceDependencyException actualException =
            await Assert.ThrowsAsync<ServiceDependencyException>(testCode: raiseTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: dependencyException);
    }

    [Fact]
    public async Task ShouldWrapUnexpectedExceptionOnRaiseEventAsync()
    {
        // Given

        Exception unexpectedException = new(message: "unexpected failure");
        EventMessage<FakeObject> message = new() { Data = new FakeObject() };

        eventServiceMock
            .Setup(expression: service => service.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: message))
            .Returns(value: ValueTask.FromException(exception: unexpectedException));

        // When

        Func<Task> raiseTask = async () => await eventProcessingService.RaiseEventAsync(
            name: "event-name",
            data: message);

        // Then

        ServiceException actualException = await Assert.ThrowsAsync<ServiceException>(
            testCode: raiseTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: unexpectedException);
    }
}