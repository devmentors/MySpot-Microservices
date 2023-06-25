using Microsoft.EntityFrameworkCore;
using MySpot.Services.Availability.Core.Entities;

namespace MySpot.Services.Availability.Infrastructure.DAL;

internal class AvailabilityDbContext : DbContext
{
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<Resource> Resources { get; set; } = null!;
        
    public AvailabilityDbContext(DbContextOptions<AvailabilityDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}