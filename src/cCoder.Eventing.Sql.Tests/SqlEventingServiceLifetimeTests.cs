using System;
using cCoder.Data;
using cCoder.Data.Models;
using cCoder.Data.Models.Eventing;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace cCoder.Eventing.Sql.Tests;

public sealed class SqlEventingServiceLifetimeTests
{
    [Fact]
    public void AddSqlEventingHostedServices_ValidatesWithScopedCoreContextFactory()
    {
        ServiceCollection services = new();
        services.AddLogging();
        services.AddScoped<ICoreContextFactory, TestCoreContextFactory>();

        services.AddSqlEventingHostedServices();

        using ServiceProvider provider = services.BuildServiceProvider(
            new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });

        using IServiceScope scope = provider.CreateScope();
        Assert.NotNull(scope.ServiceProvider.GetRequiredService<ISqlEventStorage>());
    }

    private sealed class TestCoreContextFactory : ICoreContextFactory
    {
        public CoreDataContext CreateCoreContext() =>
            throw new NotSupportedException("The context is not used by this lifetime test.");
    }
}
