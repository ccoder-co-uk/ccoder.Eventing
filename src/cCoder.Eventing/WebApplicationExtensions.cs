using Microsoft.AspNetCore.Builder;

namespace cCoder.Eventing;

public static class WebApplicationExtensions
{
    public static WebApplication StartEventingWeb(this WebApplication app) => app;

    public static WebApplication StartEventingHostedServices(this WebApplication app) => app;
}
