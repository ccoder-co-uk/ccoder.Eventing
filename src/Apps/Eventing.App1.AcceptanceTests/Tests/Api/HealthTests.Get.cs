// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using FluentAssertions;

namespace Eventing.App1.AcceptanceTests.Tests.Api;

public partial class HealthTests
{
    [Fact]
    public async Task ShouldReturnOk()
    {
        // Given

        // When

        HttpResponseMessage response =
            await fixture.Client.GetAsync(requestUri:"/Health");

        string content = await response.Content.ReadAsStringAsync();

        // Then

        response.EnsureSuccessStatusCode();

        content.Should()
            .Contain(expected:"OK");
    }
}