using Microsoft.EntityFrameworkCore;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.Types;

namespace MySpot.Services.Reservations.Infrastructure.DAL.Repositories;

internal sealed class UserRepository : IUserRepository
{
    private readonly ReservationsDbContext _context;
    private readonly DbSet<User> _users;

    public UserRepository(ReservationsDbContext context)
    {
        _users = context.Users;
        _context = context;
    }

    public Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken = default)
        => _users.SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _users.AddAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}