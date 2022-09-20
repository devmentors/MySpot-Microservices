using MySpot.Services.Availability.Core.Events;
using MySpot.Services.Availability.Core.Exceptions;
using MySpot.Services.Availability.Core.ValueObjects;

namespace MySpot.Services.Availability.Core.Entities;

public sealed class Resource : AggregateRoot
{
    private readonly ISet<Tag> _tags = new HashSet<Tag>();
    private readonly ICollection<Reservation> _reservations = new HashSet<Reservation>();
    
    public Capacity Capacity { get; } = null!;
    public IEnumerable<Tag> Tags => _tags;
    public IEnumerable<Reservation> Reservations => _reservations;

    private Resource()
    {
    }

    private Resource(AggregateId id, Capacity capacity, IEnumerable<Tag> tags)
    {
        Id = id;
        Capacity = capacity;
        _tags = tags.ToHashSet();
    }

    public static Resource Create(AggregateId id, Capacity capacity, IEnumerable<Tag> tags)
    {
        var resource = new Resource(id, capacity, tags.ToHashSet());
        resource.AddEvent(new ResourceCreated(resource));
        return resource;
    }

    public void AddReservation(Reservation reservation)
    {
        var collidingReservation = _reservations.SingleOrDefault(r => r.Date == reservation.Date);
        if (collidingReservation is not null)
        {
            if (collidingReservation.Priority >= reservation.Priority)
            {
                throw new CannotExpropriateReservationException(Id, reservation.Date);
            }

            if (collidingReservation.Capacity + reservation.Capacity > Capacity)
            {
                throw new ResourceCapacityExceededException(Id, Capacity);
            }

            _reservations.Remove(collidingReservation);
            AddEvent(new ReservationCanceled(this, collidingReservation));
        }

        _reservations.Add(reservation);
        AddEvent(new ReservationAdded(this, reservation));
    }

    public void ReleaseReservation(Reservation reservation)
    {
        if (!_reservations.Remove(reservation))
        {
            return;
        }
            
        AddEvent(new ReservationReleased(this, reservation));
    }

    public void Delete()
    {
        foreach (var reservation in Reservations)
        {
            AddEvent(new ReservationCanceled(this, reservation));
        }
            
        AddEvent(new ResourceDeleted(this));
    }
}