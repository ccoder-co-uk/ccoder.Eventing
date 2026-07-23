// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests.Foundations;

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
            await eventProviderService.RaiseEventAsync(name:null, message:inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfMessageIsNull()
    {
        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync<FakeObject>(name:"event-name", message:null);

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
            await eventProviderService.RaiseEventAsync(name:"event-name", message:inputMessage);

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
            await eventProviderService.RaiseEventAsync(name:"event-name", message:inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncIfMessagesAreNull()
    {
        IEventProviderService eventProviderService = CreateEventProviderService();

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync<FakeObject>(name:"event-name", messages:null);

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
            await eventProviderService.RaiseEventsAsync(name:null, messages:inputMessages);

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
            await eventProviderService.RaiseEventsAsync(name:"event-name", messages:inputMessages);

        await raiseEventsAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }
}