using Microsoft.EntityFrameworkCore;
using MySpot.Services.ParkingSpots.Core.Entities;

namespace MySpot.Services.ParkingSpots.Core.DAL;

internal class ParkingSpotsDbContext : DbContext
{
    public DbSet<ParkingSpot> ParkingSpots { get; set; } = null!;
        
    public ParkingSpotsDbContext(DbContextOptions<ParkingSpotsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}