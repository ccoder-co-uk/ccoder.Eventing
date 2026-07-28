// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Apps;

namespace Eventing.App1;

public partial class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args:args);
        builder.Services.AddEventingAppCommon(
            applicationConfiguration: builder.Configuration);

        WebApplication app = builder.Build();
        app.StartEventingChat();

        app.Run();
    }
}