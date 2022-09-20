using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Factories;

public interface IWeeklyReservationsFactory
{
    Task<WeeklyReservations> CreateAsync(UserId userId, Week week, CancellationToken cancellationToken = default);
}