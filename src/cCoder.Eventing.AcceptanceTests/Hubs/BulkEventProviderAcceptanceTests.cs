// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;

using cCoder.Eventing.Models;

namespace cCoder.Eventing.AcceptanceTests.Hubs;

public sealed partial class BulkEventProviderAcceptanceTests
{
    [Fact]
    public async Task ShouldExposeTypedBulkEventProviderBehavior()
    {
        // Given

        const string eventName = "test-event";
        IServiceProvider serviceProvider = new FakeServiceProvider();
        EventMessage<string>[] messages = [new()];
        int calls = 0;

        BulkEventProvider<string> provider = new()
        {
            Events = [eventName],
            Handler = (_, _) =>
            {
                calls++;
                return ValueTask.CompletedTask;
            }
        };

        // When

        bool canHandle = provider.CanHandle<string>(name: eventName);

        await provider.HandleAsync(
            serviceProvider: serviceProvider,
            messages: messages);

        // Then

        Assert.True(condition: canHandle);
        Assert.Equal(expected: 1, actual: calls);
    }

    [Fact]
    public async Task ShouldRejectMissingAndNonMatchingBulkEventProviderHandlers()
    {
        // Given

        BulkEventProvider<string> provider = new();

        // When

        bool canHandle = provider.CanHandle<int>(name: "missing");

        Func<Task> missingHandler = async () => await provider.HandleAsync(
            serviceProvider: new FakeServiceProvider(),
            messages: Array.Empty<EventMessage<string>>());

        // Then

        Assert.False(condition: canHandle);

        await Assert.ThrowsAsync<InvalidOperationException>(
            testCode: missingHandler);
    }

    private sealed class FakeServiceProvider : IServiceProvider
    {
        public object? GetService(Type serviceType) =>
            null;
    }
}