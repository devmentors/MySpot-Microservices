using MySpot.Services.Reservations.Core.Exception;
using MySpot.Services.Reservations.Core.Policies;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Entities;

public class WeeklyReservations : AggregateRoot
{
    private readonly JobTitle _jobTitle = JobTitle.None;
    private readonly HashSet<Reservation> _reservations = new();

    private readonly UserId _userId;
    private readonly Week _week;
    
    private WeeklyReservations()
    {
    }
    
    public WeeklyReservations(AggregateId id, User user, Week week)
    {
        Id = id;
        _userId = user.Id;
        _week = week;
        _jobTitle = user.JobTitle;
        IncrementVersion();
    }

    internal void AddReservation(Reservation reservation, Date now, IEnumerable<IReservationPolicy> policies)
    {
        if (reservation.Date <= now ||  reservation.Date < _week.From || reservation.Date > _week.To)
        {
            throw new InvalidReservationDateException(reservation.Date.Value);
        }
        
        if (_reservations.Any(x => x.ParkingSpotId == reservation.ParkingSpotId && x.Date == reservation.Date))
        {
            throw new ParkingSpotAlreadyReservedException(reservation.ParkingSpotId, reservation.Date);
        }
        
        var policy = policies.SingleOrDefault(p => p.CanBeApplied(_jobTitle));
        if (policy is null)
        {
            throw new NoReservationPolicyFoundException(_jobTitle);
        }

        if (!policy.CanReserve(_reservations))
        {
            throw new CannotMakeReservationException(reservation.ParkingSpotId);
        }

        _reservations.Add(reservation);
        IncrementVersion();
    }

    public void RemoveReservation(ReservationId reservationId)
    {
        var reservation = GetReservation(reservationId);
        _reservations.Remove(reservation);
        IncrementVersion();
    }

    public void RemoveReservations(IEnumerable<Reservation> reservations)
    {
        _reservations.RemoveWhere(r => reservations.Any(rr => rr.Id == r.Id));
        IncrementVersion();
    }
    
    public void ChangeReservationsNote(ReservationId reservationId, string note)
    {
        var reservation = GetReservation(reservationId);
        reservation.ChangeNote(note);
        IncrementVersion();
    }

    public void ChangeLicensePlate(ReservationId reservationId, LicensePlate licensePlate)
    {
        var reservation = GetReservation(reservationId);
        reservation.ChangeLicensePlate(licensePlate);
        IncrementVersion();
    }
    
    public void MarkReservationAsVerified(ReservationId reservationId)
    {
        var reservation = GetReservation(reservationId);
        reservation.MarkAsVerified();
        IncrementVersion();
    }
    
    public void MarkReservationAsIncorrect(ReservationId reservationId)
    {
        var reservation = GetReservation(reservationId);
        reservation.MarkAsIncorrect();
        IncrementVersion();
    }

    public bool HasAnyIncorrectReservation()
        => _reservations.Any(r => r.State == ReservationState.Incorrect);

    private Reservation GetReservation(ReservationId reservationId)
    {
        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId);

        if (reservation is null)
        {
            throw new ReservationNotFoundException(reservationId);
        }
        if (reservation.Date < Date.Now)
        {
            throw new CannotModifyPastReservationException(reservation.Date);
        }

        return reservation;
    }
}