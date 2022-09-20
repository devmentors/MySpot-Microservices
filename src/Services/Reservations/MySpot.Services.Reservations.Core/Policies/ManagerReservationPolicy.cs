using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Policies;

public sealed class ManagerReservationPolicy : IReservationPolicy
{
    public bool CanBeApplied(string jobTitle)
        => jobTitle is JobTitle.Manager;

    public bool CanReserve(IEnumerable<Reservation> reservations)
    {
        var totalEmployeeReservations = reservations.Count();
        return totalEmployeeReservations <= 4;
    }
}