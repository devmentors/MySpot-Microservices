using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.ParkingSpots.Core.Events;

[Message("parking_spots", "parking_spot_deleted")]
public record ParkingSpotDeleted(Guid ParkingSpotId) : IEvent;