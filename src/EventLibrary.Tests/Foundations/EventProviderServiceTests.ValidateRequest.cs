using EventLibrary.Models;
using EventLibrary.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace EventLibrary.Tests.Foundations;

public partial class EventProviderServiceTests
{
    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfNameIsNull()
    {
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync(null, inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfMessageIsNull()
    {
        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync<FakeObject>("event-name", null);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfDataIsNull()
    {
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = null
        };

        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync("event-name", inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfAuthInfoIsNull()
    {
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = null,
            Data = new FakeObject()
        };

        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync("event-name", inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncIfMessagesAreNull()
    {
        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync<FakeObject>("event-name", null);

        await raiseEventsAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncIfNameIsNull()
    {
        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync(null, inputMessages);

        await raiseEventsAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncIfMessageContainsInvalidData()
    {
        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = null
            }
        ];

        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync("event-name", inputMessages);

        await raiseEventsAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }
}
