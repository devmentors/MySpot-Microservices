using MySpot.Services.Users.Core.Entities;

namespace MySpot.Services.Users.Core.Repositories;

internal interface IUserRepository
{
    Task<User?> GetAsync(string email);
    Task AddAsync(User user);
}