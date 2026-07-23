// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Builder;

namespace cCoder.Eventing.Http;

public static class WebApplicationExtensions
{
    public static WebApplication StartHttpEventingWeb(this WebApplication app) =>
        app;

    public static WebApplication StartHttpEventingHostedServices(this WebApplication app) =>
        app;
}