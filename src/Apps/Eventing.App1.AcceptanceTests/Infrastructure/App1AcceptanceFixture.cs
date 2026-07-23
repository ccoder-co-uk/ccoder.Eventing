// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Net.Http;

namespace Eventing.App1.AcceptanceTests.Infrastructure;

public class App1AcceptanceFixture : IDisposable
{
    public App1AcceptanceFactory Factory { get; } = new();

    public HttpClient Client { get; }

    public App1AcceptanceFixture() =>
        Client = Factory.CreateClient();

    public void Dispose()
    {
        Client.Dispose();
        Factory.Dispose();
    }
}