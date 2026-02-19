using Microsoft.EntityFrameworkCore;
using Pos.Infrastructure.Persistence;
using Xunit;

namespace Pos.Tests;

public sealed class DatabaseSeedTests
{
    [Fact]
    public async Task Migrates_and_seeds_demo_tenant()
    {
        var connectionString = Environment.GetEnvironmentVariable("TEST_DB_CONNECTION")
            ?? "Host=localhost;Port=5432;Database=posdb;Username=pos;Password=pospass";

        var options = new DbContextOptionsBuilder<PosDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        await using var context = new PosDbContext(options);
        await context.Database.MigrateAsync();

        var tenant = await context.Tenants
            .AsNoTracking()
            .SingleOrDefaultAsync(t => t.Id == SeedData.TenantId);

        Assert.NotNull(tenant);
        Assert.Equal("Demo", tenant!.Name);
    }
}
