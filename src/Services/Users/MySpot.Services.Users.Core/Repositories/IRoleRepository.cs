using MySpot.Services.Users.Core.Entities;

namespace MySpot.Services.Users.Core.Repositories;

internal interface IRoleRepository
{
    Task<Role?> GetAsync(string name);
    Task<IReadOnlyList<Role>> GetAllAsync();
    Task AddAsync(Role role);
}