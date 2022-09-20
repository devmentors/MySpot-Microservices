using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Availability.Infrastructure.DAL;

namespace MySpot.Services.Availability.Tests.Integration;

[ExcludeFromCodeCoverage]
internal sealed class TestDatabase : IDisposable
{
    public AvailabilityDbContext Context { get; }

    public TestDatabase()
    {
        var connectionString = $"Host=localhost;Database=myspot-availability-tests-{Guid.NewGuid():N};Username=postgres;Password=";
        Context = new AvailabilityDbContext(new DbContextOptionsBuilder<AvailabilityDbContext>().UseNpgsql(connectionString).Options);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public Task InitAsync() => Context.Database.MigrateAsync();

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}