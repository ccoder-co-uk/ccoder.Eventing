using System.Text.Json;
using FluentAssertions;

namespace Eventing.App1.AcceptanceTests.Tests.Api;

public sealed partial class BaselineTests
{
    [Fact]
    public async Task Get_GivenBaselineEndpoint_ShouldReturnPackagesArray()
    {
        JsonElement baseline = await GetBaselineAsync();

        baseline.ValueKind.Should().Be(JsonValueKind.Array);
    }
}
