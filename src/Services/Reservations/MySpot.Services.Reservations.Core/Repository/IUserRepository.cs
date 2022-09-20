using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Types;

namespace MySpot.Services.Reservations.Core.Repository;

public interface IUserRepository
{
    Task<User?> GetAsync(UserId userId, CancellationToken cancellationToken = default);
    Task AddAsync(User user, CancellationToken cancellationToken = default);
}