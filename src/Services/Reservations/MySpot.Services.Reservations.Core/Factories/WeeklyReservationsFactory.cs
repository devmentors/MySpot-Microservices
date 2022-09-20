using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Exception;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Factories;

public sealed class WeeklyReservationsFactory : IWeeklyReservationsFactory
{
    private readonly IUserRepository _userRepository;

    public WeeklyReservationsFactory(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<WeeklyReservations> CreateAsync(UserId userId, Week week,
        CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetAsync(userId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(userId);
        }
        
        return new WeeklyReservations(AggregateId.Create(), user, week);
    }
}