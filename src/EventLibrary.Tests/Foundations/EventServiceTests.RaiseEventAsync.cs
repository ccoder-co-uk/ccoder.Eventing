using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Foundations;

public partial class EventServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsync()
    {
        string inputName = "event-name";
        FakeObject inputMessage = new();
        IServiceProvider inputServiceProvider = Mock.Of<IServiceProvider>();
        List<FakeObject> actualMessages = [];

        IEnumerable<Func<IServiceProvider, FakeObject, ValueTask>> handlers =
        [
            (_, message) =>
            {
                actualMessages.Add(message);
                return ValueTask.CompletedTask;
            },
            (_, message) =>
            {
                actualMessages.Add(message);
                return ValueTask.CompletedTask;
            }
        ];

        eventBrokerMock
            .Setup(broker => broker.GetHandlers(inputName))
            .Returns(handlers);

        await eventService.RaiseEventAsync(inputName, inputServiceProvider, inputMessage);

        actualMessages.Should().HaveCount(2);
        actualMessages.Should().OnlyContain(message => message == inputMessage);

        eventBrokerMock.Verify(
            broker => broker.GetHandlers(inputName),
            Times.Once);
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncWhenNoHandlersExist()
    {
        string inputName = "event-name";
        FakeObject inputMessage = new();
        IServiceProvider inputServiceProvider = Mock.Of<IServiceProvider>();

        eventBrokerMock
            .Setup(broker => broker.GetHandlers(inputName))
            .Returns(Array.Empty<Func<IServiceProvider, FakeObject, ValueTask>>());

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(inputName, inputServiceProvider, inputMessage);

        await raiseEventAsyncTask.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ShouldThrowWrappedExceptionOnRaiseEventAsyncIfBrokerFails()
    {
        string inputName = "event-name";
        FakeObject inputMessage = new();
        IServiceProvider inputServiceProvider = Mock.Of<IServiceProvider>();
        Exception innerException = new("Broker failure");

        eventBrokerMock
            .Setup(broker => broker.GetHandlers(inputName))
            .Throws(innerException);

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(inputName, inputServiceProvider, inputMessage);

        InvalidOperationException actualException =
            await Assert.ThrowsAsync<InvalidOperationException>(raiseEventAsyncTask);

        actualException.InnerException.Should().BeSameAs(innerException);
    }

    [Fact]
    public async Task ShouldThrowWrappedExceptionOnRaiseEventAsyncIfHandlerFails()
    {
        string inputName = "event-name";
        FakeObject inputMessage = new();
        IServiceProvider inputServiceProvider = Mock.Of<IServiceProvider>();
        Exception innerException = new("Handler failure");

        IEnumerable<Func<IServiceProvider, FakeObject, ValueTask>> handlers =
        [
            (_, _) => throw innerException
        ];

        eventBrokerMock
            .Setup(broker => broker.GetHandlers(inputName))
            .Returns(handlers);

        Func<Task> raiseEventAsyncTask = async () =>
            await eventService.RaiseEventAsync(inputName, inputServiceProvider, inputMessage);

        InvalidOperationException actualException =
            await Assert.ThrowsAsync<InvalidOperationException>(raiseEventAsyncTask);

        actualException.InnerException.Should().BeSameAs(innerException);
    }
}
