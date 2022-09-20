using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpot.Services.Availability.Core.Entities;
using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Infrastructure.DAL.Configurations;

internal sealed class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
{
    public void Configure(EntityTypeBuilder<Reservation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).HasConversion(x => x.Value, v => new ReservationId(v));
        builder.Property(x => x.Capacity).HasConversion(x => x.Value, v => new Capacity(v));
        builder.Property(x => x.Date).HasConversion(x => x.Value, v => new Date(v));
    }
}