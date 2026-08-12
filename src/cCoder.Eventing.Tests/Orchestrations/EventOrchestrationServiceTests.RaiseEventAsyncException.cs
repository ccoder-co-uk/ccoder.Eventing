// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Models.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Orchestrations;

public partial class EventOrchestrationServiceTests
{
    [Fact]
    public async Task ShouldWrapArgumentExceptionOnRaiseEventAsync()
    {
        // Given

        ArgumentException dependencyException = new();
        EventMessage<FakeObject> message = CreateEventMessage();

        eventProviderServiceMock
            .Setup(expression: service => service.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: message))
            .Returns(value: ValueTask.FromException<bool>(exception: dependencyException));

        // When

        Func<Task> raiseTask = async () => await eventOrchestrationService.RaiseEventAsync(
            name: "event-name",
            message: message);

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

        InvalidOperationException dependencyException = new();
        EventMessage<FakeObject> message = CreateEventMessage();

        eventProviderServiceMock
            .Setup(expression: service => service.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: message))
            .Returns(value: ValueTask.FromException<bool>(exception: dependencyException));

        // When

        Func<Task> raiseTask = async () => await eventOrchestrationService.RaiseEventAsync(
            name: "event-name",
            message: message);

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

        Exception unexpectedException = new();
        EventMessage<FakeObject> message = CreateEventMessage();

        eventProviderServiceMock
            .Setup(expression: service => service.RaiseEventAsync(
                name: It.IsAny<string>(),
                message: message))
            .Returns(value: ValueTask.FromException<bool>(exception: unexpectedException));

        // When

        Func<Task> raiseTask = async () => await eventOrchestrationService.RaiseEventAsync(
            name: "event-name",
            message: message);

        // Then

        ServiceException actualException =
            await Assert.ThrowsAsync<ServiceException>(testCode: raiseTask);

        actualException.InnerException
            .Should()
            .BeSameAs(expected: unexpectedException);
    }

    private static EventMessage<FakeObject> CreateEventMessage() =>
        new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };
}