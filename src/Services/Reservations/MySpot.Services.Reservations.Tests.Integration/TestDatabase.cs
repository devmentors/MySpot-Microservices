using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Reservations.Infrastructure.DAL;

namespace MySpot.Services.Reservations.Tests.Integration;

[ExcludeFromCodeCoverage]
internal sealed class TestDatabase : IDisposable
{
    public ReservationsWriteDbContext Context { get; }

    public TestDatabase()
    {
        var connectionString = $"Host=localhost;Database=myspot-reservations-tests-{Guid.NewGuid():N};Username=postgres;Password=";
        Context = new ReservationsWriteDbContext(new DbContextOptionsBuilder<ReservationsWriteDbContext>().UseNpgsql(connectionString).Options);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public Task InitAsync() => Context.Database.MigrateAsync();

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}