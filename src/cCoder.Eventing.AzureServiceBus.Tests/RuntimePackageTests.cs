// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using System.Reflection;
using Xunit;

namespace cCoder.Eventing.AzureServiceBus.Tests;

public sealed partial class RuntimePackageTests
{
    [Fact]
    public void RuntimeAssembly_WhenAnalyzerIsNotDeployed_UsesContractsAssembly()
    {
        // Given

        Assembly runtimeAssembly = typeof(IAzureServiceBusEventHub).Assembly;

        // When

        string[] runtimeDependencies = runtimeAssembly
            .GetReferencedAssemblies()
            .Select(selector: dependency => dependency.Name)
            .ToArray();

        // Then

        runtimeDependencies
            .Should()
            .NotContain(unexpected: "cCoder.CodeAnalysis");

        runtimeDependencies
            .Should()
            .Contain(expected: "cCoder.CodeAnalysis.Contracts");
    }
}