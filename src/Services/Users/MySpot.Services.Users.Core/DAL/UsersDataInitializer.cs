using Micro.DAL.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySpot.Services.Users.Core.Entities;

namespace MySpot.Services.Users.Core.DAL;

internal sealed class UsersDataInitializer : IDataInitializer
{
    private readonly HashSet<string> _permissions = new()
    {
        "users"
    };

    private readonly UsersDbContext _dbContext;
    private readonly ILogger<UsersDataInitializer> _logger;

    public UsersDataInitializer(UsersDbContext dbContext, ILogger<UsersDataInitializer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task InitAsync()
    {
        if (await _dbContext.Roles.AnyAsync())
        {
            return;
        }

        await AddRolesAsync();
        await _dbContext.SaveChangesAsync();
    }

    private async Task AddRolesAsync()
    {
        await _dbContext.Roles.AddAsync(new Role
        {
            Name = "admin",
            Permissions = _permissions
        });
        await _dbContext.Roles.AddAsync(new Role
        {
            Name = "user",
            Permissions = new List<string>()
        });

        _logger.LogInformation("Initialized roles.");
    }
}