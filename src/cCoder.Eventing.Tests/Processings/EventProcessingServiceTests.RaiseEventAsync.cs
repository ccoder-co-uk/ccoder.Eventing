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
    public async Task ShouldRaiseEventAsync()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject { Name = "test" }
        };

        // When

        await eventProcessingService.RaiseEventAsync(name:inputName, data:inputMessage);

        // Then

        eventServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, message:inputMessage),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowValidationExceptionWhenMessageIsNull()
    {
        // Given

        string inputName = "event-name";

        // When

        Func<Task> raiseEventAsync = async () =>
            await eventProcessingService.RaiseEventAsync(name:inputName, data:null);

        // Then

        await raiseEventAsync.Should()
            .ThrowAsync<ServiceValidationException>();
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncWhenAuthInfoIsNull()
    {
        // Given

        string inputName = "event-name";

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = null,
            Data = new FakeObject { Name = "test" }
        };

        // When

        await eventProcessingService.RaiseEventAsync(name:inputName, data:inputMessage);

        // Then

        eventServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, message:inputMessage),
times: Times.Once);
    }
}