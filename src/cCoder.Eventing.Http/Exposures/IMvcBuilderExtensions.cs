// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Http;

public static class IMvcBuilderExtensions
{
    public static IMvcBuilder AddHttpEventingControllers(this IMvcBuilder builder) =>
        builder.AddApplicationPart(
            assembly: typeof(HttpEventController).Assembly);
}