using EventLibrary.Models;
using EventLibrary.Models.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventsAsync()
    {
        string inputName = "event-name";
        EventMessage<FakeObject>[] inputMessages =
        [
            new EventMessage<FakeObject>
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            },
            new EventMessage<FakeObject>
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        await serviceBusProcessingService.RaiseEventsAsync(inputName, inputMessages);

        serviceBusServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, It.IsAny<EventMessage<FakeObject>>()),
            Times.Exactly(inputMessages.Length));
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncWhenMessageIsInvalid()
    {
        string inputName = "event-name";
        EventMessage<FakeObject>[] inputMessages =
        [
            new EventMessage<FakeObject>
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            },
            new EventMessage<FakeObject>
            {
                AuthInfo = null,
                Data = new FakeObject()
            }
        ];

        Func<Task> raiseEventsAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventsAsync(inputName, inputMessages);

        await raiseEventsAsyncTask.Should().ThrowAsync<InvalidOperationException>();

        serviceBusServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, It.IsAny<EventMessage<FakeObject>>()),
            Times.Once);
    }
}
