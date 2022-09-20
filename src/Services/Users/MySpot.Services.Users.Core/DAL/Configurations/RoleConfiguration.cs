using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpot.Services.Users.Core.Entities;

namespace MySpot.Services.Users.Core.DAL.Configurations;

internal class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(x => x.Name);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(100);
        builder
            .Property(x => x.Permissions)
            .HasConversion(x => string.Join(',', x), x => x.Split(',', StringSplitOptions.None));
            
        builder
            .Property(x => x.Permissions).Metadata.SetValueComparer(
                new ValueComparer<IEnumerable<string>>(
                    (c1, c2) => c2 != null && c1 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, next) => HashCode.Combine(a, next.GetHashCode()))));
    }
}