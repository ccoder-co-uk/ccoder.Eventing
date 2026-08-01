// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Eventing.App1.AcceptanceTests.Infrastructure;
using FluentAssertions;

namespace Eventing.App1.AcceptanceTests.Tests.Api;

public partial class HealthAcceptanceTests
{
    [Fact]
    public async Task ShouldReturnOk()
    {
        // Given

        using App1AcceptanceFixture fixture = new();

        // When

        HttpResponseMessage response =
            await fixture.Client.GetAsync(requestUri: "/Health");

        string content = await response.Content.ReadAsStringAsync();

        // Then

        response.EnsureSuccessStatusCode();

        content.Should()
            .Contain(expected: "OK");
    }
}