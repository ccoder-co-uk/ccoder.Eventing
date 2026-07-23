// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;

namespace Eventing.App1.AcceptanceTests.Tests.Api;

public partial class ToolsTests
{
    [Fact]
    public async Task ShouldReturnToolingUi()
    {
        // Given

        // When

        HttpResponseMessage response =
            await fixture.Client.GetAsync(requestUri:"/tools/index.html");

        string content = await response.Content.ReadAsStringAsync();

        // Then

        response.EnsureSuccessStatusCode();

        content.Should()
            .Contain(expected:"Eventing Chat");
    }
}