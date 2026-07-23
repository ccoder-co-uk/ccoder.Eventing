// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Eventing.App1.AcceptanceTests.Infrastructure;

namespace Eventing.App1.AcceptanceTests.Tests.Api;

public partial class HealthTests(App1AcceptanceFixture fixture)
    : IClassFixture<App1AcceptanceFixture>
{
    private readonly App1AcceptanceFixture fixture = fixture;
}