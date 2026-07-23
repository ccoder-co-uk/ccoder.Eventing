// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using Microsoft.AspNetCore.Builder;

namespace cCoder.Eventing.AzureServiceBus;

public static class WebApplicationExtensions
{
    public static WebApplication StartAzureServiceBusEventingWeb(this WebApplication app) =>
        app;

    public static WebApplication StartAzureServiceBusEventingHostedServices(this WebApplication app) =>
        app;
}