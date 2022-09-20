using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Core.Entities;

public class Reservation
{
    public ReservationId Id { get; } = null!;
    public Capacity Capacity { get; } = null!;
    public Date Date { get; } = null!;
    public int Priority { get; }

    private Reservation()
    {
    }

    public Reservation(ReservationId id, Capacity capacity, Date date, int priority)
        => (Id, Capacity, Date, Priority) = (id, capacity, date, priority);
}