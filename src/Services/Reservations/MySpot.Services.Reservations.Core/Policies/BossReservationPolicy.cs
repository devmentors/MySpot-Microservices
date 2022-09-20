using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Policies;

public class BossReservationPolicy : IReservationPolicy
{
    public bool CanBeApplied(string jobTitle)
        => jobTitle is JobTitle.Boss;

    public bool CanReserve(IEnumerable<Reservation> reservations)
        => true;
}