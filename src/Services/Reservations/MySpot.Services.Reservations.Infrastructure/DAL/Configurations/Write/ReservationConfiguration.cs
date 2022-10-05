using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Infrastructure.DAL.Configurations;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        var parkingSpotIdConverter = new ValueConverter<ParkingSpotId, Guid>
            (x => x.Value, v => new ParkingSpotId(v));
        
        var capacityConverter = new ValueConverter<Capacity, int>
            (x => x.Value, v => new Capacity(v));
        
        var licencePlateConverter = new ValueConverter<LicensePlate, string>
            (x => x.Value, v => new LicensePlate(v));

        var dateConverter = new ValueConverter<Date, DateTimeOffset>
            (x => x.Value, v => new Date(v));

        var stateConverter = new ValueConverter<ReservationState, string>
            (x => x.Value, v => new ReservationState(v));

        
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, v => new ReservationId(v));
        
        builder.Property(typeof(ParkingSpotId), "ParkingSpotId")
            .HasConversion(parkingSpotIdConverter)
            .HasColumnName("ParkingSpotId");
        
        builder.Property(typeof(Capacity), "Capacity")
            .HasConversion(capacityConverter)
            .HasColumnName("Capacity");
        
        builder.Property(typeof(LicensePlate), "LicensePlate")
            .HasConversion(licencePlateConverter)
            .HasColumnName("LicensePlate"); 
        
        builder.Property(typeof(Date), "Date")
            .HasConversion(dateConverter)
            .HasColumnName("Date"); 
        
        builder.Property(typeof(ReservationState), "State")
            .HasConversion(stateConverter)
            .HasColumnName("State"); 
   }
}