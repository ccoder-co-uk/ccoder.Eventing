using System.Net;
using System.Text.Json;
using Eventing.App1.AcceptanceTests.Infrastructure;
using FluentAssertions;

namespace Eventing.App1.AcceptanceTests.Tests.Api;

public sealed partial class BaselineTests(App1AcceptanceFixture fixture)
    : IClassFixture<App1AcceptanceFixture>
{
    private readonly App1AcceptanceFixture fixture = fixture;

    private async Task<JsonElement> GetBaselineAsync()
    {
        using HttpResponseMessage response = await fixture.Client.GetAsync("/Api/Eventing/Baseline");
        string content = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK, content);
        return JsonDocument.Parse(content).RootElement.Clone();
    }
}
