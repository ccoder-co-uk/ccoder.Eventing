using FluentAssertions;

namespace Eventing.App1.AcceptanceTests.Tests.Api;

public partial class ToolsTests
{
    [Fact]
    public async Task ShouldReturnToolingUi()
    {
        HttpResponseMessage response =
            await fixture.Client.GetAsync("/tools/index.html");

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Eventing Chat");
    }
}
