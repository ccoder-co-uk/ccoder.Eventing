// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Processings;

public partial class EventProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsync()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject { Name = "test" }
        };

        await eventProcessingService.RaiseEventAsync(inputName, inputMessage);

        eventServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessage),
            Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncWhenMessageIsNull()
    {
        string inputName = "event-name";

        await eventProcessingService.RaiseEventAsync(inputName, null);

        eventServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, null),
            Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncWhenAuthInfoIsNull()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = null,
            Data = new FakeObject { Name = "test" }
        };

        await eventProcessingService.RaiseEventAsync(inputName, inputMessage);

        eventServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessage),
            Times.Once);
    }
}