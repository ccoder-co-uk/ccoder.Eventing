using cCoder.Eventing.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

public partial class EventServiceProviderServiceTests
{
    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfNameIsNull()
    {
        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync(null, inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfMessageIsNull()
    {
        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync<FakeObject>("event-name", null);

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

        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync("event-name", inputMessage);

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

        Func<Task> raiseEventAsyncTask = async () =>
            await eventServiceProviderService.RaiseEventAsync("event-name", inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }
}
