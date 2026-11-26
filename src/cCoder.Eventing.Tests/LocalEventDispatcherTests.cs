// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading.Tasks;
using cCoder.Eventing.Brokers;
using cCoder.Eventing.Models.Exceptions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Eventing.Tests;

public sealed partial class LocalEventDispatcherTests
{
    [Fact]
    public async Task DispatchAwaitsMatchingHandlersAndStopsOnVeto()
    {
        // Given
        const string eventName = "tenant_delete";
        const string tenantId = "tenant-1";
        TaskCompletionSource completion = new(TaskCreationOptions.RunContinuationsAsynchronously);
        LocalEventRejectedException rejection = new(code: "linked_apps");
        Mock<ILocalEventHandler<string>> first = new(MockBehavior.Strict);
        Mock<ILocalEventHandler<string>> second = new(MockBehavior.Strict);
        Mock<ILocalEventHandler<string>> third = new(MockBehavior.Strict);
        Mock<ILocalEventHandler<string>> unrelated = new(MockBehavior.Strict);

        first.Setup(expression: handler => handler.HandleAsync(data: tenantId))
            .Returns(value: new ValueTask(task: completion.Task));

        second.Setup(expression: handler => handler.HandleAsync(data: tenantId))
            .Returns(value: ValueTask.FromException(exception: rejection));

        ServiceCollection services = new();
        services.AddEventing();
        services.AddKeyedSingleton<ILocalEventHandler<string>>(serviceKey: eventName, implementationInstance: first.Object);
        services.AddKeyedSingleton<ILocalEventHandler<string>>(serviceKey: eventName, implementationInstance: second.Object);
        services.AddKeyedSingleton<ILocalEventHandler<string>>(serviceKey: eventName, implementationInstance: third.Object);
        services.AddKeyedSingleton<ILocalEventHandler<string>>(serviceKey: "tenant_deleted", implementationInstance: unrelated.Object);
        using ServiceProvider provider = services.BuildServiceProvider();
        using IServiceScope scope = provider.CreateScope();
        ILocalEventBroker dispatcher = scope.ServiceProvider.GetRequiredService<ILocalEventBroker>();

        // When
        Task dispatched = dispatcher
            .DispatchAsync(name: eventName, data: tenantId)
            .AsTask();

        second.VerifyNoOtherCalls();
        completion.SetResult();

        // Then
        LocalEventRejectedException actual = await Assert.ThrowsAsync<LocalEventRejectedException>(
            testCode: () => dispatched);

        Assert.Same(expected: rejection, actual: actual);
        third.VerifyNoOtherCalls();
        unrelated.VerifyNoOtherCalls();
    }
}