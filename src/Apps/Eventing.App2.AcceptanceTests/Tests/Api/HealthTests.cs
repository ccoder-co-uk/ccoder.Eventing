// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Eventing.App2.AcceptanceTests.Infrastructure;

namespace Eventing.App2.AcceptanceTests.Tests.Api;

public partial class HealthTests(App2AcceptanceFixture fixture)
    : IClassFixture<App2AcceptanceFixture>
{
    private readonly App2AcceptanceFixture fixture = fixture;
}