using cCoder.Eventing.Apps;

namespace Eventing.App1;

public partial class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        ChatApplication.Configure(builder);

        WebApplication app = builder.Build();
        ChatApplication.Start(app);

        app.Run();
    }
}
