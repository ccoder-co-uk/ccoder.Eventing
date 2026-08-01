// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Eventing.App2.AcceptanceTests.Infrastructure;
using FluentAssertions;

namespace Eventing.App2.AcceptanceTests.Tests.Api;

public partial class ToolsAcceptanceTests
{
    [Fact]
    public async Task ShouldReturnToolingUi()
    {
        // Given

        using App2AcceptanceFixture fixture = new();

        // When

        HttpResponseMessage response =
            await fixture.Client.GetAsync(requestUri: "/tools/index.html");

        string content = await response.Content.ReadAsStringAsync();

        // Then

        response.EnsureSuccessStatusCode();

        content.Should()
            .Contain(expected: "Eventing Chat");
    }
}