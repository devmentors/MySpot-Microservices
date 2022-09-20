using Micro.Exceptions;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Exception;

public sealed class ParkingSpotAlreadyReservedException : CustomException
{
    public Guid ParkingSpotId { get; }
    public DateTimeOffset Date { get; }

    public ParkingSpotAlreadyReservedException(Guid parkingSpotId, Date date)
        : base($"Parking spot with ID: {parkingSpotId} is already reserved for date: {date}.")
    {
        ParkingSpotId = parkingSpotId;
        Date = date;
    }
}