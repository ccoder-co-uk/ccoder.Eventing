// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http;

namespace Eventing.App2.AcceptanceTests.Infrastructure;

public class App2AcceptanceFixture : IDisposable
{
    public App2AcceptanceFactory Factory { get; } = new();

    public HttpClient Client { get; }

    public App2AcceptanceFixture() =>
        Client = Factory.CreateClient();

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}