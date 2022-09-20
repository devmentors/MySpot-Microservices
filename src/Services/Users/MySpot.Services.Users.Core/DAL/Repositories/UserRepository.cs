using Microsoft.EntityFrameworkCore;
using MySpot.Services.Users.Core.Entities;
using MySpot.Services.Users.Core.Repositories;

namespace MySpot.Services.Users.Core.DAL.Repositories;

internal class UserRepository : IUserRepository
{
    private readonly UsersDbContext _context;
    private readonly DbSet<User> _users;

    public UserRepository(UsersDbContext context)
    {
        _context = context;
        _users = _context.Users;
    }

    public  Task<User?> GetAsync(string email)
        => _users.Include(x => x.Role).SingleOrDefaultAsync(x => x.Email == email);

    public async Task AddAsync(User user)
    {
        await _users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}