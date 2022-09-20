using Micro.Exceptions;

namespace MySpot.Services.Reservations.Core.Exception;

public class EmptyParkingSpotException : CustomException
{
    public EmptyParkingSpotException() : base("Parking spot name cannot be empty.")
    {
    }
}