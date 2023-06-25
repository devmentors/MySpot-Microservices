using Microsoft.EntityFrameworkCore;
using MySpot.Services.Notifications.Api.Entities;

namespace MySpot.Services.Notifications.Api.DAL;

internal sealed class NotificationsDbContext : DbContext
{
    public DbSet<Template> Templates { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
        
    public NotificationsDbContext(DbContextOptions<NotificationsDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}