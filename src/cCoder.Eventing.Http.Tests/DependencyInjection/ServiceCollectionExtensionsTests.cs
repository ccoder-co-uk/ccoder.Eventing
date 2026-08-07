// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Models;
using cCoder.Eventing.Http.Services.Processings;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using Xunit;

namespace cCoder.Eventing.Http.Tests.DependencyInjection;

public partial class ServiceCollectionExtensionsTests
{
    [Fact]
    public void ShouldRegisterEveryEntryPointAndExposure()
    {
        // Given

        HttpEventingOptions configured = new()
        {
            MaxConcurrency = 4
        };

        Action<HttpEventingOptions> configure = options =>
            options.MaxConcurrency = 2;

        ServiceCollection services = new();

        // When

        services.AddLogging();
        services.AddHttpEventingWeb(configure: configure);
        services.AddHttpEventingWeb(configuration: configured);
        services.AddHttpEventingHostedServices(configure: configure);
        services.AddHttpEventingHostedServices(configuration: configured);

        services.AddSingleton(
            implementationInstance: Mock.Of<IHttpEventProcessingService>());

        using ServiceProvider provider = services.BuildServiceProvider();

        // Then

        provider
            .GetRequiredService<IHttpEventHub>()
            .Should()
            .NotBeNull();

        services
            .Any(predicate: descriptor =>
                descriptor.ServiceType == typeof(IHostedService))
            .Should()
            .BeTrue();

        provider
            .GetRequiredService<HttpEventingOptions>()
            .Should()
            .NotBeNull();
    }

    [Fact]
    public void ShouldRejectNullConfigurations()
    {
        // Given

        ServiceCollection services = new();

        // When

        Action addWeb = () => services.AddHttpEventingWeb(
            configuration: (HttpEventingOptions)null);

        Action addHosted = () => services.AddHttpEventingHostedServices(
            configuration: (HttpEventingOptions)null);

        // Then

        addWeb
            .Should()
            .Throw<ArgumentNullException>();

        addHosted
            .Should()
            .Throw<ArgumentNullException>();
    }
}