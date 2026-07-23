// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;

namespace Eventing.App2.AcceptanceTests.Tests.Api;

public partial class ToolsTests
{
    [Fact]
    public async Task ShouldReturnToolingUi()
    {
        HttpResponseMessage response =
            await fixture.Client.GetAsync(requestUri:"/tools/index.html");

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(expected:"Eventing Chat");
    }
}