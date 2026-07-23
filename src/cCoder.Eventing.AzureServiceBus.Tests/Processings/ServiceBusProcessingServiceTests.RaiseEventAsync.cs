// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Services.Processings;
using cCoder.Eventing.AzureServiceBus.Models;
using FluentAssertions;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.Processings;

public partial class ServiceBusProcessingServiceTests
{
    [Fact]
    public async Task ShouldRaiseEventAsyncWithData()
    {
        string inputName = "event-name";
        FakeObject inputData = new() { Name = "test" };
        ServiceBusEventMessage<FakeObject> actualMessage = null;

        eventAuthInfoMock.SetupGet(auth => auth.SSOUserId).Returns("user");

        serviceBusServiceMock
            .Setup(service => service.RaiseEventAsync(
                inputName,
                It.IsAny<ServiceBusEventMessage<FakeObject>>()))
            .Callback<string, ServiceBusEventMessage<FakeObject>>((_, message) => actualMessage = message)
            .Returns(ValueTask.CompletedTask);

        await serviceBusProcessingService.RaiseEventAsync(inputName, inputData);

        actualMessage.Should().NotBeNull();
        actualMessage.Data.Should().BeSameAs(inputData);
        actualMessage.AuthInfo.SSOUserId.Should().Be("user");
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithDataIfNameIsNull()
    {
        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync(null, new FakeObject());

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithDataIfDataIsNull()
    {
        eventAuthInfoMock.SetupGet(auth => auth.SSOUserId).Returns("user");

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync("event-name", default(FakeObject));

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithDataIfAuthInfoIsNull()
    {
        ServiceBusProcessingService serviceBusProcessingServiceWithNullAuth = new(
            () => null,
            serviceBusServiceMock.Object);

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingServiceWithNullAuth.RaiseEventAsync("event-name", new FakeObject());

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncWithMessage()
    {
        string inputName = "event-name";
        ServiceBusEventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject()
        };

        await serviceBusProcessingService.RaiseEventAsync(inputName, inputMessage);

        serviceBusServiceMock.Verify(
            service => service.RaiseEventAsync(inputName, inputMessage),
            Times.Once);
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithMessageIfNameIsNull()
    {
        ServiceBusEventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject()
        };

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync(null, inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithMessageIfMessageIsNull()
    {
        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync("event-name", default(FakeObject));

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithMessageIfDataIsNull()
    {
        ServiceBusEventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = null
        };

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync("event-name", inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithMessageIfAuthInfoIsNull()
    {
        ServiceBusEventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = null,
            Data = new FakeObject()
        };

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync("event-name", inputMessage);

        await raiseEventAsyncTask.Should().ThrowAsync<InvalidOperationException>();
    }
}