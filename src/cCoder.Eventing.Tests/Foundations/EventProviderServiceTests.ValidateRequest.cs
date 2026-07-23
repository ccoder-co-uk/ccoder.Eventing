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
        // Given

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = new FakeObject()
        };

        IEventProviderService eventProviderService = CreateEventProviderService();

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync(name:null, message:inputMessage);

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfMessageIsNull()
    {
        // Given

        IEventProviderService eventProviderService = CreateEventProviderService();

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync<FakeObject>(name:"event-name", message:null);

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfDataIsNull()
    {
        // Given

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = Mock.Of<IEventAuthInfo>(),
            Data = null
        };

        IEventProviderService eventProviderService = CreateEventProviderService();

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync(name:"event-name", message:inputMessage);

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncIfAuthInfoIsNull()
    {
        // Given

        EventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = null,
            Data = new FakeObject()
        };

        IEventProviderService eventProviderService = CreateEventProviderService();

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await eventProviderService.RaiseEventAsync(name:"event-name", message:inputMessage);

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncIfMessagesAreNull()
    {
        // Given

        IEventProviderService eventProviderService = CreateEventProviderService();

        // When

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync<FakeObject>(name:"event-name", messages:null);

        // Then

        await raiseEventsAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncIfNameIsNull()
    {
        // Given

        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = new FakeObject()
            }
        ];

        IEventProviderService eventProviderService = CreateEventProviderService();

        // When

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync(name:null, messages:inputMessages);

        // Then

        await raiseEventsAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventsAsyncIfMessageContainsInvalidData()
    {
        // Given

        EventMessage<FakeObject>[] inputMessages =
        [
            new()
            {
                AuthInfo = Mock.Of<IEventAuthInfo>(),
                Data = null
            }
        ];

        IEventProviderService eventProviderService = CreateEventProviderService();

        // When

        Func<Task> raiseEventsAsyncTask = async () =>
            await eventProviderService.RaiseEventsAsync(name:"event-name", messages:inputMessages);

        // Then

        await raiseEventsAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }
}