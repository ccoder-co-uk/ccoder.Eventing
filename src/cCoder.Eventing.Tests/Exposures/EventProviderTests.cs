// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace cCoder.Eventing.Tests.Exposures;

public partial class EventProviderTests
{
    [Fact]
    public async Task ShouldExposeAndInvokeTypedHandlers()
    {
        // Given

        const string eventName = "test-event";
        IServiceProvider serviceProvider = new FakeServiceProvider();
        EventMessage<FakeObject> message = new();
        int sendCalls = 0;
        int receiveCalls = 0;

        EventProvider<FakeObject> provider = new()
        {
            Events = [eventName],
            SendHandler = (_, _, _) =>
            {
                sendCalls++;
                return ValueTask.CompletedTask;
            },
            ReceiveHandler = (_, _, _) =>
            {
                receiveCalls++;
                return ValueTask.CompletedTask;
            }
        };

        // When

        bool canSend = provider.CanSend<FakeObject>(name: eventName);
        bool canReceive = provider.CanReceive<FakeObject>(name: eventName);
        bool canReceiveUntyped = provider.CanReceive(name: eventName);

        await provider.HandleSendAsync(
            serviceProvider: serviceProvider,
            eventName: eventName,
            message: message);

        await provider.ReceiveAsync(
            serviceProvider: serviceProvider,
            eventName: eventName,
            message: message);

        // Then

        canSend
            .Should()
            .BeTrue();

        canReceive
            .Should()
            .BeTrue();

        canReceiveUntyped
            .Should()
            .BeTrue();

        provider.DataType
            .Should()
            .Be(expected: typeof(FakeObject));

        sendCalls
            .Should()
            .Be(expected: 1);

        receiveCalls
            .Should()
            .Be(expected: 1);
    }

    [Fact]
    public async Task ShouldRejectMissingHandlers()
    {
        // Given

        const string eventName = "test-event";
        IServiceProvider serviceProvider = new FakeServiceProvider();
        EventMessage<FakeObject> message = new();

        EventProvider<FakeObject> provider = new()
        {
            Events = [eventName]
        };

        // When

        Func<Task> missingSend = async () => await provider.HandleSendAsync(
            serviceProvider: serviceProvider,
            eventName: eventName,
            message: message);

        Func<Task> missingReceive = async () => await provider.HandleReceiveAsync(
            serviceProvider: serviceProvider,
            eventName: eventName,
            message: message);

        // Then

        await missingSend
            .Should()
            .ThrowAsync<InvalidOperationException>();

        await missingReceive
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public void ShouldRejectNonMatchingEventProviderRequests()
    {
        // Given

        EventProvider<FakeObject> provider = new();

        // When

        bool canReceive = provider.CanReceive(name: "missing");
        bool canSendTyped = provider.CanSend<string>(name: "missing");
        bool canReceiveTyped = provider.CanReceive<string>(name: "missing");

        // Then

        canReceive
            .Should()
            .BeFalse();

        canSendTyped
            .Should()
            .BeFalse();

        canReceiveTyped
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task ShouldAdaptLegacyHandlersWithoutReplacingExplicitHandlers()
    {
        // Given

        int legacyCalls = 0;
        int explicitCalls = 0;
        EventProvider<FakeObject> provider = new();

        PropertyInfo legacyProperty = typeof(EventProvider<FakeObject>)
            .GetProperty(name: "Handler");

        Func<IServiceProvider, EventMessage<FakeObject>, ValueTask> legacyHandler =
            (_, _) =>
            {
                legacyCalls++;
                return ValueTask.CompletedTask;
            };

        // When

        legacyProperty.SetValue(obj: provider, value: legacyHandler);

        await provider.SendHandler(
            arg1: new FakeServiceProvider(),
            arg2: "event",
            arg3: new EventMessage<FakeObject>());

        provider.SendHandler = (_, _, _) =>
        {
            explicitCalls++;
            return ValueTask.CompletedTask;
        };

        legacyProperty.SetValue(obj: provider, value: legacyHandler);

        Func<IServiceProvider, EventMessage<FakeObject>, ValueTask> storedHandler =
            (Func<IServiceProvider, EventMessage<FakeObject>, ValueTask>)
                legacyProperty.GetValue(obj: provider);

        await provider.SendHandler(
            arg1: new FakeServiceProvider(),
            arg2: "event",
            arg3: new EventMessage<FakeObject>());

        // Then

        legacyCalls
            .Should()
            .Be(expected: 1);

        explicitCalls
            .Should()
            .Be(expected: 1);

        storedHandler
            .Should()
            .BeSameAs(expected: legacyHandler);
    }

    private sealed class FakeServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType) =>
            null;
    }
}