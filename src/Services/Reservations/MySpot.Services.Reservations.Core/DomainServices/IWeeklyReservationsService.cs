using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.DomainServices;

public interface IWeeklyReservationsService
{
    Reservation Reserve(WeeklyReservations currentReservations, WeeklyReservations? lastWeekReservations,
        ParkingSpotId parkingSpotId, Capacity capacity, LicensePlate licensePlate, Date date, string? note = null);
}