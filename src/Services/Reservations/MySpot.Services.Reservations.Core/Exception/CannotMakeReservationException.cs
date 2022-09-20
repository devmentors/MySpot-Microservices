using Micro.Exceptions;
using MySpot.Services.Reservations.Core.Types;

namespace MySpot.Services.Reservations.Core.Exception;

public sealed class CannotMakeReservationException : CustomException
{
    public ParkingSpotId ParkingSpotId { get; }

    public CannotMakeReservationException(ParkingSpotId parkingSpotId) 
        : base($"Cannot reserve parking spot with ID: {parkingSpotId} due to reservation policy.")
    {
        ParkingSpotId = parkingSpotId;
    }
}