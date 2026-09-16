// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.CodeAnalysis.Exposures;
using cCoder.Eventing.Brokers;
using cCoder.Eventing.Brokers.Loggings;
using cCoder.Eventing.Models;
using cCoder.Eventing.Services.Foundations;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Eventing.Tests;

public sealed partial class ArchitectureOwnershipTests
{
    [Fact]
    public async Task EventProviderBrokerShouldForwardToExactProvider()
    {
        // Given

        const string eventName = "event-name";
        ServiceCollection services = new();
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        EventMessage<FakeObject> message = new() { Data = new FakeObject() };
        IServiceProvider actualProvider = null;
        EventMessage<FakeObject> actualMessage = null;

        EventProvider<FakeObject> provider = new()
        {
            Events = [eventName],
            SendHandler = (scope, _, value) =>
            {
                actualProvider = scope;
                actualMessage = value;

                return ValueTask.CompletedTask;
            }
        };

        IEventProviderBroker broker = new EventProviderBroker(
            configuration: new EventProviderBrokerConfiguration
            {
                CanSend = provider.CanSend,
                HandleSendAsync = provider.HandleSendAsync
            });

        // When

        bool canSend = broker.CanSend<FakeObject>(name: eventName);

        await broker.HandleSendAsync(
            serviceProvider: serviceProvider,
            eventName: eventName,
            message: message);

        // Then

        canSend
            .Should()
            .BeTrue();

        actualProvider
            .Should()
            .BeSameAs(expected: serviceProvider);

        actualMessage
            .Should()
            .BeSameAs(expected: message);
    }

    [Fact]
    public async Task BulkEventProviderBrokerShouldForwardToExactProvider()
    {
        // Given

        const string eventName = "event-name";
        ServiceCollection services = new();
        using ServiceProvider serviceProvider = services.BuildServiceProvider();
        EventMessage<FakeObject>[] messages = [new() { Data = new FakeObject() }];
        IServiceProvider actualProvider = null;
        EventMessage<FakeObject>[] actualMessages = null;

        BulkEventProvider<FakeObject> provider = new()
        {
            Events = [eventName],
            Handler = (scope, values) =>
            {
                actualProvider = scope;
                actualMessages = values;

                return ValueTask.CompletedTask;
            }
        };

        IBulkEventProviderBroker broker = new BulkEventProviderBroker(
            configuration: new BulkEventProviderBrokerConfiguration
            {
                CanHandle = provider.CanHandle,
                HandleAsync = provider.HandleAsync
            });

        // When

        bool canHandle = broker.CanHandle<FakeObject>(name: eventName);

        await broker.HandleAsync(
            serviceProvider: serviceProvider,
            messages: messages);

        // Then

        canHandle
            .Should()
            .BeTrue();

        actualProvider
            .Should()
            .BeSameAs(expected: serviceProvider);

        actualMessages
            .Should()
            .BeSameAs(expected: messages);
    }

    [Fact]
    public void ShouldRegisterOneAdapterForEachConfiguredProvider()
    {
        // Given

        EventProvider provider = new EventProvider<FakeObject>();
        BulkEventProvider bulkProvider = new BulkEventProvider<FakeObject>();
        ServiceCollection services = new();

        services.AddLogging();

        EventingConfiguration configuration = new()
        {
            EventProviders = [provider],
            BulkEventProviders = [bulkProvider]
        };

        services.AddEventing(eventingConfiguration: configuration);

        // When

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        IEnumerable<IEventProviderBroker> providerBrokers =
            serviceProvider.GetServices<IEventProviderBroker>();

        IEnumerable<IBulkEventProviderBroker> bulkProviderBrokers =
            serviceProvider.GetServices<IBulkEventProviderBroker>();

        // Then

        providerBrokers
            .Should()
            .ContainSingle();

        bulkProviderBrokers
            .Should()
            .ContainSingle();
    }

    [Fact]
    public void CrossCuttingBrokerContractsShouldBeUtilities()
    {
        // Given

        Type serviceProviderBrokerType = typeof(IServiceProviderBroker);
        Type eventAuthorizationBrokerType = typeof(IEventAuthorizationBroker);
        Type loggingBrokerType = typeof(ILoggingBroker);

        // When

        bool serviceProviderIsUtility =
            typeof(IUtilityBroker).IsAssignableFrom(
                c: serviceProviderBrokerType);

        bool eventAuthorizationIsUtility =
            typeof(IUtilityBroker).IsAssignableFrom(
                c: eventAuthorizationBrokerType);

        bool loggingIsUtility =
            typeof(IUtilityBroker).IsAssignableFrom(
                c: loggingBrokerType);

        // Then

        serviceProviderIsUtility
            .Should()
            .BeTrue();

        eventAuthorizationIsUtility
            .Should()
            .BeTrue();

        loggingIsUtility
            .Should()
            .BeTrue();
    }

    [Fact]
    public void EventAuthorizationShouldBeIsolatedByScope()
    {
        // Given

        ServiceCollection services = new();
        services.AddLogging();
        services.AddEventingForType<FakeObject>();
        services.AddEventing();

        using ServiceProvider root = services.BuildServiceProvider();

        IServiceProviderBroker broker =
            root.GetRequiredService<IServiceProviderBroker>();

        EventMessage<FakeObject> first = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = "first" },
            Data = new FakeObject()
        };

        EventMessage<FakeObject> second = new()
        {
            AuthInfo = new EventAuthInfo { SSOUserId = "second" },
            Data = new FakeObject()
        };

        // When

        using IServiceScope firstScope = broker.GetScopeForEvent(
            eventMessage: first);

        using IServiceScope secondScope = broker.GetScopeForEvent(
            eventMessage: second);

        string firstUserId = firstScope.ServiceProvider
            .GetRequiredService<IEventAuthorizationService>()
            .GetEventAuthInfo()
            .SSOUserId;

        string secondUserId = secondScope.ServiceProvider
            .GetRequiredService<IEventAuthorizationService>()
            .GetEventAuthInfo()
            .SSOUserId;

        // Then

        firstUserId
            .Should()
            .Be(expected: "first");

        secondUserId
            .Should()
            .Be(expected: "second");
    }
}