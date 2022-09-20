using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Infrastructure.DAL.Configurations;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, v => new ReservationId(v));
        builder.Property(x => x.ParkingSpotId)
            .HasConversion(x => x.Value, v => new ParkingSpotId(v));
        builder.Property(x => x.Capacity)
            .HasConversion(x => x.Value, v => new Capacity(v));
        builder.Property(x => x.LicensePlate)
            .HasConversion(x => x.Value, v => new LicensePlate(v));
        builder.Property(x => x.Date)
            .HasConversion(x => x.Value, v => new Date(v));
        builder.Property(x => x.State)
            .HasConversion(x => x.Value, v => new ReservationState(v));
    }
}