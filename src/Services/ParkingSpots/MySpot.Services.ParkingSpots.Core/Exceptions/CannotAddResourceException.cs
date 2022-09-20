using Micro.Exceptions;

namespace MySpot.Services.ParkingSpots.Core.Exceptions;

internal class CannotAddResourceException : CustomException
{
    public Guid ResourceId { get; }

    public CannotAddResourceException(Guid resourceId) : base($"Resource with ID: '{resourceId}' cannot be added.")
    {
        ResourceId = resourceId;
    }
}