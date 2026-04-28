using cCoder.Eventing.Http.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Http;

public static partial class IServiceCollectionExtensions
{
    public static IMvcBuilder AddHttpEventingControllers(this IMvcBuilder builder) =>
        builder.AddApplicationPart(typeof(HttpEventController).Assembly);
}
