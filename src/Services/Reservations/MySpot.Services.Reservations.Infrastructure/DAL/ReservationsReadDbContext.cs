using Microsoft.EntityFrameworkCore;
using MySpot.Services.Reservations.Infrastructure.DAL.Configurations.Read;
using MySpot.Services.Reservations.Infrastructure.DAL.ReadModels;

namespace MySpot.Services.Reservations.Infrastructure.DAL;

internal class ReservationsReadDbContext : DbContext
{
    public DbSet<ReservationReadModel> Reservations { get; set; } = null!;
    public DbSet<WeeklyReservationsReadModel> WeeklyReservations { get; set; } = null!;
        
    public ReservationsReadDbContext(DbContextOptions<ReservationsReadDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration<ReservationReadModel>(new ReadModelConfiguration());
        modelBuilder.ApplyConfiguration<WeeklyReservationsReadModel>(new ReadModelConfiguration());
    }
}