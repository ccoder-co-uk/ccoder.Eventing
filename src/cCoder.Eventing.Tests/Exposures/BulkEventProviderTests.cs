// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using FluentAssertions;
using Xunit;

namespace cCoder.Eventing.Tests.Exposures;

public partial class BulkEventProviderTests
{
    [Fact]
    public async Task ShouldIdentifyAndInvokeTypedHandler()
    {
        // Given

        const string eventName = "test-event";
        IServiceProvider serviceProvider = new FakeServiceProvider();
        EventMessage<FakeObject>[] messages = [new()];
        int calls = 0;

        BulkEventProvider<FakeObject> provider = new()
        {
            Events = [eventName],
            Handler = (_, _) =>
            {
                calls++;
                return ValueTask.CompletedTask;
            }
        };

        // When

        bool canHandle = provider.CanHandle<FakeObject>(name: eventName);

        await provider.HandleAsync(
            serviceProvider: serviceProvider,
            messages: messages);

        // Then

        canHandle
            .Should()
            .BeTrue();

        calls
            .Should()
            .Be(expected: 1);
    }

    [Fact]
    public async Task ShouldRejectMissingAndNonMatchingHandlers()
    {
        // Given

        BulkEventProvider<FakeObject> provider = new();
        IServiceProvider serviceProvider = new FakeServiceProvider();

        // When

        bool canHandle = provider.CanHandle<string>(name: "missing");

        Func<Task> missingHandler = async () => await provider.HandleAsync(
            serviceProvider: serviceProvider,
            messages: Array.Empty<EventMessage<FakeObject>>());

        // Then

        canHandle
            .Should()
            .BeFalse();

        await missingHandler
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    private sealed class FakeServiceProvider : IServiceProvider
    {
        public object GetService(Type serviceType) =>
            null;
    }
}