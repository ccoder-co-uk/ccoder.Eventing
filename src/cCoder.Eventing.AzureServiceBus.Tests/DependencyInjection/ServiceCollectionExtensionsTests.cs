// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.AzureServiceBus.Models;
using cCoder.Eventing.AzureServiceBus.Services.Processings;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests.DependencyInjection;

public partial class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldRegisterEveryEntryPointAndExposure()
    {
        // Given

        const string connectionString =
            "Endpoint=sb://fake/;SharedAccessKeyName=fake;SharedAccessKey=fake=";

        AzureServiceBusEventingConfiguration configured = new()
        {
            ConnectionString = connectionString
        };

        Action<AzureServiceBusEventingConfiguration> configure = configuration =>
            configuration.ConnectionString = connectionString;

        ServiceCollection services = new();

        // When

        services.AddLogging();
        services.AddAzureServiceBusEventingWeb(configure: configure);
        services.AddAzureServiceBusEventingWeb(configuration: configured);
        services.AddAzureServiceBusEventingHostedServices(configure: configure);
        services.AddAzureServiceBusEventingHostedServices(configuration: configured);
        services.AddAzureServiceBusEventing(configure: configure);

        services.AddAzureServiceBusEventing(
            serviceBusConnectionString: connectionString);

        services.AddSingleton(
            implementationInstance: Mock.Of<IServiceBusProcessingService>());

        using ServiceProvider provider = services.BuildServiceProvider();

        // Then

        provider
            .GetRequiredService<IAzureServiceBusEventHub>()
            .Should()
            .NotBeNull();

        provider
            .GetServices<AzureServiceBusEventingConfiguration>()
            .Should()
            .NotBeEmpty();
    }

    [Fact]
    public async Task ShouldResolveEventHubFromRegisteredServicesAsync()
    {
        // Given

        const string connectionString =
            "Endpoint=sb://fake/;SharedAccessKeyName=fake;SharedAccessKey=fake=";

        ServiceCollection services = new();
        services.AddLogging();

        services.AddAzureServiceBusEventing(
            serviceBusConnectionString: connectionString);

        await using ServiceProvider provider = services.BuildServiceProvider();

        // When

        Action resolveEventHub = () =>
            provider.GetRequiredService<IAzureServiceBusEventHub>();

        // Then

        resolveEventHub
            .Should()
            .NotThrow();
    }

    [Fact]
    public void ShouldRejectNullConfigurations()
    {
        // Given

        ServiceCollection services = new();

        // When

        Action addWeb = () => services.AddAzureServiceBusEventingWeb(
            configuration: (AzureServiceBusEventingConfiguration)null);

        Action addHosted = () => services.AddAzureServiceBusEventingHostedServices(
            configuration: (AzureServiceBusEventingConfiguration)null);

        // Then

        addWeb
            .Should()
            .Throw<ArgumentNullException>();

        addHosted
            .Should()
            .Throw<ArgumentNullException>();
    }
}