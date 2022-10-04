using Micro.Time;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Exception;
using MySpot.Services.Reservations.Core.Policies;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.DomainServices;

public sealed class WeeklyReservationsService : IWeeklyReservationsService
{
    private readonly IEnumerable<IReservationPolicy> _policies;
    private readonly IClock _clock;

    public WeeklyReservationsService(IEnumerable<IReservationPolicy> policies, IClock clock)
    {
        _policies = policies;
        _clock = clock;
    }
    
    public Reservation Reserve(WeeklyReservations currentReservations, WeeklyReservations? lastWeekReservations,
        ParkingSpotId parkingSpotId, Capacity capacity, LicensePlate licensePlate, Date date, string? note = null)
    {
        if (lastWeekReservations?.HasAnyIncorrectReservation() is true)
        {
            throw new CannotMakeReservationException(parkingSpotId);
        }

        var reservation = new Reservation(ReservationId.Create(), parkingSpotId, capacity, licensePlate, date, note);
        currentReservations.AddReservation(reservation, new Date(_clock.Current()), _policies);

        return reservation;
    }
}