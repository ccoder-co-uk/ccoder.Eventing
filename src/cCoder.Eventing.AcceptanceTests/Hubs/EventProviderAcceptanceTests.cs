// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Reflection;
using System.Threading.Tasks;

using cCoder.Eventing.Models;

namespace cCoder.Eventing.AcceptanceTests.Hubs;

public sealed partial class EventProviderAcceptanceTests
{
    [Fact]
    public async Task ShouldExposeTypedEventProviderBehavior()
    {
        // Given

        const string eventName = "test-event";
        IServiceProvider serviceProvider = new FakeServiceProvider();
        EventMessage<string> message = new();
        int sendCalls = 0;
        int receiveCalls = 0;

        EventProvider<string> provider = new()
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

        bool canSend = provider.CanSend<string>(name: eventName);
        bool canReceive = provider.CanReceive<string>(name: eventName);
        bool canReceiveUntyped = provider.CanReceive(name: eventName);

        await provider.HandleSendAsync(
            serviceProvider: serviceProvider,
            eventName: eventName,
            message: message);

        await provider.ReceiveAsync(
            serviceProvider: serviceProvider,
            eventName: eventName,
            eventMessage: message);

        // Then

        Assert.True(condition: canSend);
        Assert.True(condition: canReceive);
        Assert.True(condition: canReceiveUntyped);
        Assert.Equal(expected: typeof(string), actual: provider.DataType);
        Assert.Equal(expected: 1, actual: sendCalls);
        Assert.Equal(expected: 1, actual: receiveCalls);
    }

    [Fact]
    public async Task ShouldRejectMissingAndNonMatchingEventProviderHandlers()
    {
        // Given

        EventProvider<string> provider = new();
        IServiceProvider serviceProvider = new FakeServiceProvider();
        EventMessage<string> message = new();

        // When

        bool canReceive = provider.CanReceive(name: "missing");
        bool canSendTyped = provider.CanSend<int>(name: "missing");
        bool canReceiveTyped = provider.CanReceive<int>(name: "missing");

        Func<Task> missingSend = async () => await provider.HandleSendAsync(
            serviceProvider: serviceProvider,
            eventName: "missing",
            message: message);

        Func<Task> missingReceive = async () => await provider.HandleReceiveAsync(
            serviceProvider: serviceProvider,
            eventName: "missing",
            message: message);

        // Then

        Assert.False(condition: canReceive);
        Assert.False(condition: canSendTyped);
        Assert.False(condition: canReceiveTyped);

        await Assert.ThrowsAsync<InvalidOperationException>(
            testCode: missingSend);

        await Assert.ThrowsAsync<InvalidOperationException>(
            testCode: missingReceive);
    }

    [Fact]
    public async Task ShouldAdaptLegacyEventProviderHandlers()
    {
        // Given

        int legacyCalls = 0;
        int explicitCalls = 0;
        EventProvider<string> provider = new();

        PropertyInfo legacyProperty = typeof(EventProvider<string>)
            .GetProperty(name: "Handler") ??
                throw new InvalidOperationException(message: "Handler property was not found.");

        Func<IServiceProvider, EventMessage<string>, ValueTask> legacyHandler =
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
            arg3: new EventMessage<string>());

        provider.SendHandler = (_, _, _) =>
        {
            explicitCalls++;
            return ValueTask.CompletedTask;
        };

        legacyProperty.SetValue(obj: provider, value: legacyHandler);

        await provider.SendHandler(
            arg1: new FakeServiceProvider(),
            arg2: "event",
            arg3: new EventMessage<string>());

        // Then

        Assert.Equal(expected: 1, actual: legacyCalls);
        Assert.Equal(expected: 1, actual: explicitCalls);
        Assert.Same(expected: legacyHandler, actual: legacyProperty.GetValue(obj: provider));
    }

    private sealed class FakeServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) =>
            null;
    }
}