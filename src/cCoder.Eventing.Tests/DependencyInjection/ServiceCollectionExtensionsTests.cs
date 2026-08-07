// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Models;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Eventing.Tests.DependencyInjection;

public partial class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldRegisterEveryEntryPointAndProviderExposure()
    {
        // Given

        EventProvider eventProvider = new EventProvider<FakeObject>
        {
            Events = ["single"],
            ReceiveHandler = (_, _, _) => ValueTask.CompletedTask
        };

        BulkEventProvider bulkEventProvider = new BulkEventProvider<FakeObject>
        {
            Events = ["bulk"],
            Handler = (_, _) => ValueTask.CompletedTask
        };

        EventingConfiguration configured = new()
        {
            EventProviders = [eventProvider],
            BulkEventProviders = [bulkEventProvider]
        };

        Action<EventingConfiguration> configure = configuration =>
        {
            configuration.EventProviders = [eventProvider];
            configuration.BulkEventProviders = [bulkEventProvider];
        };

        ServiceCollection services = new();

        // When

        services.AddLogging();
        services.AddEventing();
        services.AddEventing(configure: configure);
        services.AddEventing(eventingConfiguration: configured);
        services.AddEventingWeb(configure: configure);
        services.AddEventingWeb(configuration: configured);
        services.AddEventingHostedServices(configure: configure);
        services.AddEventingHostedServices(configuration: configured);
        services.AddEventProviders(eventProviders: [eventProvider, null]);
        services.AddBulkEventProviders(bulkEventProviders: [bulkEventProvider, null]);
        services.AddEventProviders(eventProviders: null);
        services.AddBulkEventProviders(bulkEventProviders: null);
        services.AddEventingForType<FakeObject>();

        using ServiceProvider provider = services.BuildServiceProvider();

        // Then

        provider
            .GetRequiredService<IEventHub>()
            .Should()
            .NotBeNull();

        provider
            .GetServices<EventProvider>()
            .Should()
            .Contain(expected: eventProvider);

        provider
            .GetServices<BulkEventProvider>()
            .Should()
            .Contain(expected: bulkEventProvider);
    }

    [Fact]
    public void ShouldRejectNullConfigurations()
    {
        // Given

        ServiceCollection services = new();

        // When

        Action addWeb = () => services.AddEventingWeb(
            configuration: (EventingConfiguration)null);

        Action addHosted = () => services.AddEventingHostedServices(
            configuration: (EventingConfiguration)null);

        Action addEventing = () => services.AddEventing(
            eventingConfiguration: null);

        // Then

        addWeb
            .Should()
            .Throw<ArgumentNullException>();

        addHosted
            .Should()
            .Throw<ArgumentNullException>();

        addEventing
            .Should()
            .Throw<ArgumentNullException>();
    }
}