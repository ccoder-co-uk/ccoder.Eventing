// ---------------------------------------------------------------
// Copyright (c) Paul.Ward@ccoder.co.uk
// ---------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace cCoder.Eventing.Sql.Dependencies;

internal sealed class SqlEventWorkerScopeDependency(IServiceScopeFactory scopeFactory)
    : IAsyncDisposable
{
    private IServiceScope scope;

    public async Task RunScopedAsync<TService>(Func<TService, IServiceProvider, Task> operation)
        where TService : notnull
    {
        scope = scopeFactory.CreateScope();

        try
        {
            TService service = scope.ServiceProvider.GetRequiredService<TService>();
            await operation(arg1: service, arg2: scope.ServiceProvider);
        }
        finally
        {
            scope.Dispose();
            scope = null;
        }
    }

    public ValueTask DisposeAsync()
    {
        scope?.Dispose();
        scope = null;
        return ValueTask.CompletedTask;
    }
}