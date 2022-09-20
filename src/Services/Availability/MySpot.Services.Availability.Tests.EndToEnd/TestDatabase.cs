using System;
using System.Diagnostics.CodeAnalysis;
using Micro.DAL.Postgres;
using Micro.Testing;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Availability.Infrastructure.DAL;

namespace MySpot.Services.Availability.Tests.EndToEnd;

[ExcludeFromCodeCoverage]
internal sealed class TestDatabase : IDisposable
{
    public AvailabilityDbContext Context { get; }

    public TestDatabase()
    {
        var options = new OptionsProvider().Get<PostgresOptions>("postgres");
        Context = new AvailabilityDbContext(new DbContextOptionsBuilder<AvailabilityDbContext>().UseNpgsql(options.ConnectionString).Options);
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Context.Dispose();
    }
}