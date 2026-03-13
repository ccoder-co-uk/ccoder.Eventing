using EventLibrary.Models;
using EventLibrary.Services.Processings;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    [Fact]
    public async Task ShouldRethrowOnRaiseEventAsyncIfProcessingServiceFails()
    {
        string inputName = "event-name";
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };
        Exception innerException = new("Processing failure");

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(inputName, (_, _) => ValueTask.CompletedTask);

        eventProcessingServiceMock
            .Setup(service => service.RaiseEventAsync(inputName, inputMessage))
            .Returns(new ValueTask(Task.FromException(innerException)));

        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync(inputName, inputMessage);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(raiseEventAsyncTask);

        actualException.Should().BeSameAs(innerException);
    }

    [Fact]
    public async Task ShouldRethrowOnRaiseEventsAsyncIfProcessingServiceFails()
    {
        string inputName = "event-name";
        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];
        Exception innerException = new("Processing failure");

        serviceProviderBrokerMock
            .Setup(broker => broker.GetService<IEventProcessingService<FakeObject>>())
            .Returns(eventProcessingServiceMock.Object);

        eventServiceProviderService.ListenToEvent<FakeObject>(inputName, (_, _) => ValueTask.CompletedTask);

        eventProcessingServiceMock
            .Setup(service => service.RaiseEventAsync(inputName, inputMessages[0]))
            .Returns(new ValueTask(Task.FromException(innerException)));

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventsAsync(inputName, inputMessages);

        Exception actualException =
            await Assert.ThrowsAsync<Exception>(raiseEventsAsyncTask);

        actualException.Should().BeSameAs(innerException);
    }
}
