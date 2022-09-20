using Micro.Transactions.Inbox;
using Micro.Transactions.Outbox;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.ParkingSpots.Core.Entities;

namespace MySpot.Services.ParkingSpots.Core.DAL;

internal class ParkingSpotsDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; } = null!;
    public DbSet<OutboxMessage> Outbox { get; set; } = null!;
    public DbSet<ParkingSpot> ParkingSpots { get; set; } = null!;
        
    public ParkingSpotsDbContext(DbContextOptions<ParkingSpotsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}