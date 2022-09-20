using Micro.Exceptions;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Exception;

public sealed class InvalidWeeklyParkingSpotDatesException : CustomException
{
    public InvalidWeeklyParkingSpotDatesException(ParkingSpotId parkingSpotId, Date from, Date to) 
        : base($"Parking spot with ID: {parkingSpotId} cannot define hours from {from} to {to}")
    {
    }
}