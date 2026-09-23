// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;
using System.Reflection;
using Xunit;

namespace cCoder.Eventing.Tests;

public sealed partial class RuntimePackageTests
{
    [Fact]
    public void RuntimeAssembly_WhenAnalyzerIsNotDeployed_DoesNotRequireAnalyzerAssembly()
    {
        // Given

        Assembly runtimeAssembly = typeof(IEventHub).Assembly;

        // When

        AssemblyName[] runtimeDependencies =
            runtimeAssembly.GetReferencedAssemblies();

        // Then

        runtimeDependencies
            .Select(selector: dependency => dependency.Name)
            .Should()
            .NotContain(unexpected: "cCoder.CodeAnalysis");

        runtimeDependencies
            .Select(selector: dependency => dependency.Name)
            .Should()
            .Contain(expected: "cCoder.CodeAnalysis.Contracts");
    }
}