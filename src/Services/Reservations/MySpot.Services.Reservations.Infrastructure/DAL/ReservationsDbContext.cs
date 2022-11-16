using Microsoft.EntityFrameworkCore;
using MySpot.Services.Reservations.Core.Entities;

namespace MySpot.Services.Reservations.Infrastructure.DAL;

internal class ReservationsDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
        
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}