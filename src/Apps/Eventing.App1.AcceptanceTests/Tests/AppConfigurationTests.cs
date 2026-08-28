// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;

namespace Eventing.App1.AcceptanceTests.Tests;

public sealed partial class AppConfigurationTests
{
    [Fact]
    public void App1_ShouldUseStandardRootConfigurationType()
    {
        // Given
        Type commonAppMarker =
            typeof(cCoder.Eventing.Apps.IServiceCollectionExtensions);

        // When
        Type? appConfigurationType = commonAppMarker.Assembly
            .GetType(name: "cCoder.Eventing.Apps.Models.AppConfiguration");

        // Then
        appConfigurationType
            .Should()
            .NotBeNull();
    }
}