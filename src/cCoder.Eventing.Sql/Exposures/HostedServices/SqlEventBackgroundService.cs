// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System.Threading;
using System.Threading.Tasks;
using cCoder.Eventing.Sql.Services.Processings;
using cCoder.Eventing.Sql.Dependencies;
using Microsoft.Extensions.Hosting;

namespace cCoder.Eventing.Sql.Exposures.HostedServices;

internal sealed class SqlEventBackgroundService(SqlEventWorkerScopeDependency scopeDependency)
    : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken) =>
        scopeDependency.RunScopedAsync<ISqlEventProcessingService>(
            operation: (service, provider) => service.ProcessAsync(
                scopedProvider: provider,
                cancellationToken: stoppingToken));
}