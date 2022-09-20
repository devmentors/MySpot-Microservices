using Micro.Exceptions;

namespace MySpot.Services.ParkingSpots.Core.Exceptions;

internal class ParkingSpotNotFoundException : CustomException
{
    public Guid ParkingSpotId { get; }

    public ParkingSpotNotFoundException(Guid parkingSpotId) : base($"Parking spot with ID: '{parkingSpotId}' was not found.")
    {
        ParkingSpotId = parkingSpotId;
    }
}