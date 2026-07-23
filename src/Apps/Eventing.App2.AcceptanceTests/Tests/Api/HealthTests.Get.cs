// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;

namespace Eventing.App2.AcceptanceTests.Tests.Api;

public partial class HealthTests
{
    [Fact]
    public async Task ShouldReturnOk()
    {
        HttpResponseMessage response =
            await fixture.Client.GetAsync(requestUri:"/Health");

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain(expected:"OK");
    }
}