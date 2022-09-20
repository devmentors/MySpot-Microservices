using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Entities;

public class Reservation
{
    public ReservationId Id { get; private set; } = null!;
    public ParkingSpotId ParkingSpotId { get; private set; } = null!;
    public Capacity Capacity { get; private set;} = null!;
    public LicensePlate LicensePlate { get; private set; } = null!;
    public Date Date { get; private set; }  = null!;
    public string? Note { get; private set; }
    public ReservationState State { get; private set; } = null!;

    private Reservation()
    {
    }
    
    internal Reservation(ReservationId id, ParkingSpotId parkingSpotId, Capacity capacity, 
        LicensePlate licensePlate, Date date, string? note  = null)
    {
        Id = id;
        ParkingSpotId = parkingSpotId;
        Capacity = capacity;
        LicensePlate = licensePlate;
        Date = date;
        Note = note;
        State = ReservationState.Unverified;
    }

    internal void ChangeNote(string note)
        => Note = note;
    
    internal void ChangeLicensePlate(LicensePlate licensePlate)
        => LicensePlate = licensePlate;

    internal void MarkAsVerified()
        => State = ReservationState.Verified;
    
    internal void MarkAsIncorrect()
        => State = ReservationState.Incorrect;
}