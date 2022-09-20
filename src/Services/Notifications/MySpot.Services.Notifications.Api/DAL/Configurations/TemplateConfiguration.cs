using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpot.Services.Notifications.Api.Entities;

namespace MySpot.Services.Notifications.Api.DAL.Configurations;

internal class TemplateConfiguration : IEntityTypeConfiguration<Template>
{
    public void Configure(EntityTypeBuilder<Template> builder)
    {
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x => x.Name).IsRequired();
        builder.Property(x => x.Title).IsRequired();
        builder.Property(x => x.Body).IsRequired();
    }
}