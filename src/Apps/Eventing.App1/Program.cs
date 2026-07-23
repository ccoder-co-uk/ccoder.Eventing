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
        ChatApplication.Configure(builder:builder);

        WebApplication app = builder.Build();
        ChatApplication.Start(app:app);

        app.Run();
    }
}