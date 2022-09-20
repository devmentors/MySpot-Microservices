using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Types;

namespace MySpot.Services.Reservations.Core.Repository;

public interface IWeeklyReservationsRepository
{
    Task<WeeklyReservations?> GetForLastWeekAsync(UserId userId, CancellationToken cancellationToken = default);
    Task<WeeklyReservations?> GetForCurrentWeekAsync(UserId userId, CancellationToken cancellationToken = default);
    Task AddAsync(WeeklyReservations weeklyReservations, CancellationToken cancellationToken = default);
    Task UpdateAsync(WeeklyReservations weeklyReservations, CancellationToken cancellationToken = default);
}