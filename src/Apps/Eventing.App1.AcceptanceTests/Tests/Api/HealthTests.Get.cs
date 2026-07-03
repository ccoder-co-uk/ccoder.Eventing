using FluentAssertions;

namespace Eventing.App1.AcceptanceTests.Tests.Api;

public partial class HealthTests
{
    [Fact]
    public async Task ShouldReturnOk()
    {
        HttpResponseMessage response =
            await fixture.Client.GetAsync("/Health");

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("OK");
    }
}
