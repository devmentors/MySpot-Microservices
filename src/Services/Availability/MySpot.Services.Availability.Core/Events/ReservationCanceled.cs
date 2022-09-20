using MySpot.Services.Availability.Core.Entities;

namespace MySpot.Services.Availability.Core.Events;

public class ReservationCanceled : IDomainEvent
{
    public Resource Resource { get; }
    public Reservation Reservation { get; }

    public ReservationCanceled(Resource resource, Reservation reservation)
        => (Resource, Reservation) = (resource, reservation);
}