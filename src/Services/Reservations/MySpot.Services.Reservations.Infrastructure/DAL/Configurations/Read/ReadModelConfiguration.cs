using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Infrastructure.DAL.ReadModels;

namespace MySpot.Services.Reservations.Infrastructure.DAL.Configurations.Read;

internal sealed class ReadModelConfiguration : IEntityTypeConfiguration<WeeklyReservationsReadModel>, 
    IEntityTypeConfiguration<ReservationReadModel>
{
    public void Configure(EntityTypeBuilder<WeeklyReservationsReadModel> builder)
    {
        builder.ToTable("WeeklyReservations");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.To).HasColumnName("Week");
        
        builder
            .HasMany(x => x.Reservations)
            .WithOne(x => x.WeeklyReservations);
    }

    public void Configure(EntityTypeBuilder<ReservationReadModel> builder)
    {
        builder.ToTable("Reservations");
    }
}