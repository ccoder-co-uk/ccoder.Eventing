// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps.Exposures;
using cCoder.Eventing.Apps.Services.Orchestrations;
using cCoder.Eventing.Http.Controllers;
using cCoder.Eventing.Http.Services.Processings;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace cCoder.Eventing.Http.Tests;

public sealed partial class ArchitectureDependencyTests
{
    [Fact]
    public void HttpEventController_WhenConstructed_ConsumesProcessingServiceDirectly()
    {
        // Given

        ConstructorInfo constructor = typeof(HttpEventController)
            .GetConstructors()
            .Single();

        // When

        Type[] dependencies = constructor
            .GetParameters()
            .Select(selector: parameter => parameter.ParameterType)
            .ToArray();

        // Then

        dependencies
            .Should()
            .Contain(expected: typeof(IHttpEventProcessingService));

        dependencies
            .Should()
            .NotContain(unexpected: typeof(IHttpEventHub));
    }

    [Fact]
    public void ChatOrchestrationServiceContract_WhenDeclared_DoesNotInheritExposureContract()
    {
        // Given

        Type orchestrationContract = typeof(IChatOrchestrationService);

        // When

        Type[] inheritedContracts = orchestrationContract.GetInterfaces();

        // Then

        inheritedContracts
            .Should()
            .NotContain(unexpected: typeof(IChatManager));
    }
}