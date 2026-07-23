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
        // Given

        string inputName = "event-name";
        FakeObject inputData = new() { Name = "test" };
        ServiceBusEventMessage<FakeObject> actualMessage = null;

        eventAuthInfoMock.SetupGet(expression:auth => auth.SSOUserId)
            .Returns(value:"user");

        serviceBusServiceMock
            .Setup(expression:service => service.RaiseEventAsync(
                inputName,
                It.IsAny<ServiceBusEventMessage<FakeObject>>()))
            .Callback<string, ServiceBusEventMessage<FakeObject>>(action:(_, message) => actualMessage = message)
            .Returns(value:ValueTask.CompletedTask);

        // When

        await serviceBusProcessingService.RaiseEventAsync(name:inputName, data:inputData);

        // Then

        actualMessage.Should()
            .NotBeNull();

        actualMessage.Data.Should()
            .BeSameAs(expected:inputData);

        actualMessage.AuthInfo.SSOUserId.Should()
            .Be(expected:"user");
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithDataIfNameIsNull()
    {
        // Given

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync(name:null, data:new FakeObject());

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithDataIfDataIsNull()
    {
        // Given

        eventAuthInfoMock.SetupGet(expression:auth => auth.SSOUserId)
            .Returns(value:"user");

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync(name:"event-name", data:default(FakeObject));

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithDataIfAuthInfoIsNull()
    {
        // Given

        ServiceBusProcessingService serviceBusProcessingServiceWithNullAuth = new(
            () => null,
            serviceBusServiceMock.Object);

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingServiceWithNullAuth.RaiseEventAsync(name:"event-name", data:new FakeObject());

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldRaiseEventAsyncWithMessage()
    {
        // Given

        string inputName = "event-name";

        ServiceBusEventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject()
        };

        // When

        await serviceBusProcessingService.RaiseEventAsync(name:inputName, message:inputMessage);

        // Then

        serviceBusServiceMock.Verify(
expression: service => service.RaiseEventAsync(name:inputName, eventMessage:inputMessage),
times: Times.Once);
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithMessageIfNameIsNull()
    {
        // Given

        ServiceBusEventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = new FakeObject()
        };

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync(name:null, message:inputMessage);

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithMessageIfMessageIsNull()
    {
        // Given

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync(name:"event-name", data:default(FakeObject));

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithMessageIfDataIsNull()
    {
        // Given

        ServiceBusEventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = new ServiceBusEventAuthInfo { SSOUserId = "user" },
            Data = null
        };

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync(name:"event-name", message:inputMessage);

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task ShouldThrowOnRaiseEventAsyncWithMessageIfAuthInfoIsNull()
    {
        // Given

        ServiceBusEventMessage<FakeObject> inputMessage = new()
        {
            AuthInfo = null,
            Data = new FakeObject()
        };

        // When

        Func<Task> raiseEventAsyncTask = async () =>
            await serviceBusProcessingService.RaiseEventAsync(name:"event-name", message:inputMessage);

        // Then

        await raiseEventAsyncTask.Should()
            .ThrowAsync<InvalidOperationException>();
    }
}