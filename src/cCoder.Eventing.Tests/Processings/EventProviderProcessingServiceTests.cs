// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Models.Exceptions;
using cCoder.Eventing.Services.Foundations;
using cCoder.Eventing.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Processings;

public sealed partial class EventProviderProcessingServiceTests
{
    [Fact]
    public async Task ShouldClassifyEventProviderFoundationFailures()
    {
        // Given

        const string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            Data = new FakeObject()
        };

        Exception[] dependencyExceptions =
        [
            new ArgumentException(message: "validation failure"),
            new InvalidOperationException(message: "dependency failure"),
            new Exception(message: "unexpected failure")
        ];

        Type[] expectedExceptionTypes =
        [
            typeof(ServiceValidationException),
            typeof(ServiceDependencyException),
            typeof(ServiceException)
        ];

        Mock<IEventProviderService> foundationServiceMock = new();

        EventProviderProcessingService service = new(
            eventProviderService: foundationServiceMock.Object);

        // When

        List<Exception> actualExceptions = [];

        foreach (Exception dependencyException in dependencyExceptions)
        {
            foundationServiceMock
                .Setup(expression: dependency => dependency.RaiseEventAsync(
                    name: inputName,
                    message: inputMessage))
                .Returns(value: ValueTask.FromException<bool>(
                    exception: dependencyException));

            Exception actualException = await Record.ExceptionAsync(
                testCode: async () => await service.RaiseEventAsync(
                    name: inputName,
                    message: inputMessage));

            actualExceptions.Add(item: actualException);
        }

        // Then

        actualExceptions
            .Select(selector: exception => exception.GetType())
            .Should()
            .Equal(expected: expectedExceptionTypes);

        actualExceptions
            .Select(selector: exception => exception.InnerException)
            .Should()
            .Equal(expected: dependencyExceptions);
    }

    [Fact]
    public async Task ShouldClassifyBulkEventProviderFoundationFailures()
    {
        // Given

        const string inputName = "event-name";

        EventMessage<FakeObject>[] inputMessages =
        [
            new EventMessage<FakeObject>
            {
                Data = new FakeObject()
            }
        ];

        Exception[] dependencyExceptions =
        [
            new ArgumentException(message: "validation failure"),
            new InvalidOperationException(message: "dependency failure"),
            new Exception(message: "unexpected failure")
        ];

        Type[] expectedExceptionTypes =
        [
            typeof(ServiceValidationException),
            typeof(ServiceDependencyException),
            typeof(ServiceException)
        ];

        Mock<IBulkEventProviderService> foundationServiceMock = new();

        BulkEventProviderProcessingService service = new(
            eventProviderService: foundationServiceMock.Object);

        // When

        List<Exception> actualExceptions = [];

        foreach (Exception dependencyException in dependencyExceptions)
        {
            foundationServiceMock
                .Setup(expression: dependency => dependency.RaiseEventsAsync(
                    name: inputName,
                    messages: inputMessages))
                .Returns(value: ValueTask.FromException<bool>(
                    exception: dependencyException));

            Exception actualException = await Record.ExceptionAsync(
                testCode: async () => await service.RaiseEventsAsync(
                    name: inputName,
                    messages: inputMessages));

            actualExceptions.Add(item: actualException);
        }

        // Then

        actualExceptions
            .Select(selector: exception => exception.GetType())
            .Should()
            .Equal(expected: expectedExceptionTypes);

        actualExceptions
            .Select(selector: exception => exception.InnerException)
            .Should()
            .Equal(expected: dependencyExceptions);
    }
}