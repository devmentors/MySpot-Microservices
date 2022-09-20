using MySpot.Services.Reservations.Core.Entities;

namespace MySpot.Services.Reservations.Core.Policies;

public interface IReservationPolicy
{
    bool CanBeApplied(string jobTitle);
    bool CanReserve(IEnumerable<Reservation> reservations);
}