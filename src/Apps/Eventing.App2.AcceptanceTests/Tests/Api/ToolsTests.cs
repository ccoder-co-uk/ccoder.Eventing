using Eventing.App2.AcceptanceTests.Infrastructure;

namespace Eventing.App2.AcceptanceTests.Tests.Api;

public partial class ToolsTests(App2AcceptanceFixture fixture)
    : IClassFixture<App2AcceptanceFixture>
{
    private readonly App2AcceptanceFixture fixture = fixture;
}
