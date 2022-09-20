using Micro.Transactions.Inbox;
using Micro.Transactions.Outbox;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Reservations.Core.Entities;

namespace MySpot.Services.Reservations.Infrastructure.DAL;

internal class ReservationsDbContext : DbContext
{
    public DbSet<InboxMessage> Inbox { get; set; } = null!;
    public DbSet<OutboxMessage> Outbox { get; set; } = null!;
    public DbSet<Reservation> Reservations { get; set; } = null!;
    public DbSet<WeeklyReservations> WeeklyReservations { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
        
    public ReservationsDbContext(DbContextOptions<ReservationsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}