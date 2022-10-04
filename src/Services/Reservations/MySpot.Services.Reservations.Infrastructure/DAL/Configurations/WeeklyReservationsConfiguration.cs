using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Infrastructure.DAL.Configurations;

internal sealed class WeeklyReservationsConfiguration : IEntityTypeConfiguration<WeeklyReservations>
{
    public void Configure(EntityTypeBuilder<WeeklyReservations> builder)
    {
        var jobTitleConverter = new ValueConverter<JobTitle, string>(x => x.Value,
            x => new JobTitle(x));
        
        var userIdConverter = new ValueConverter<UserId, Guid>
            (x => x.Value, v => new UserId(v));
        
        var weekConverter = new ValueConverter<Week, DateTimeOffset>
            (x => x.To.Value, v => new Week(v));

        builder.HasKey(x => x.Id);
        builder.HasOne<User>().WithMany().HasForeignKey("_userId");

        builder.Property(x => x.Id)
            .HasConversion(x => x.Value, v => new AggregateId(v));
      
        builder.Property(typeof(UserId), "_userId")
            .HasConversion(userIdConverter)
            .HasColumnName("UserId");
        
        builder.Property(typeof(Week), "_week")
            .HasConversion(weekConverter)
            .HasColumnName("Week");

        builder.Property(typeof(JobTitle), "_jobTitle")
            .HasConversion(jobTitleConverter)
            .HasColumnName(nameof(JobTitle));

        builder.HasMany("_reservations");

    }
}