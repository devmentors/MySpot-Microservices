using Micro.DAL.Postgres;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Notifications.Api.Entities;

namespace MySpot.Services.Notifications.Api.DAL;

internal sealed class NotificationsDataInitializer : IDataInitializer
{
    private readonly NotificationsDbContext _dbContext;
    private readonly ILogger<NotificationsDataInitializer> _logger;

    public NotificationsDataInitializer(NotificationsDbContext dbContext, ILogger<NotificationsDataInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if (await _dbContext.Templates.AnyAsync())
        {
            return;
        }

        await AddTemplatesAsync();
        await _dbContext.SaveChangesAsync();
    }

    private async Task AddTemplatesAsync()
    {
        await _dbContext.Templates.AddAsync(new Template(Guid.NewGuid(), "account_created", "Account created",
            "Your account has been created"));
        await _dbContext.Templates.AddAsync(new Template(Guid.NewGuid(), "reservation_added", "Reservation added",
            "Reservation has been added"));
        _logger.LogInformation("Initialized templates.");
    }
}