// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using cCoder.Eventing.Http.Dependencies;
using cCoder.Eventing.Http.Models;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Http;

public static partial class IServiceCollectionExtensions
{
    public static void AddHttpEventingWeb(
        this IServiceCollection services,
        Action<HttpEventingOptions> configure = null) =>
        ServiceCollectionDependency.AddHttpEventing(
            services: services,
            configure: configure);

    public static void AddHttpEventingHostedServices(
        this IServiceCollection services,
        Action<HttpEventingOptions> configure = null) =>
        ServiceCollectionDependency.AddHttpEventingHostedServices(
            services: services,
            configure: configure);

    public static void AddHttpEventing(
        this IServiceCollection services,
        Action<HttpEventingOptions> configure = null) =>
        ServiceCollectionDependency.AddHttpEventing(
            services: services,
            configure: configure);
}