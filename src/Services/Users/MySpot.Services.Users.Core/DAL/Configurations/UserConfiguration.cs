using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpot.Services.Users.Core.Entities;

namespace MySpot.Services.Users.Core.DAL.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(x => x.Email).IsUnique();
        builder.Property(x => x.Email).IsRequired().HasMaxLength(500);
        builder.Property(x => x.Password).IsRequired().HasMaxLength(500);
        builder.Property(x => x.JobTitle).IsRequired().HasMaxLength(500);
    }
}