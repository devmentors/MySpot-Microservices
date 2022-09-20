using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.ParkingSpots.Core.Events;

[Message("parking_spots", "parking_spot_created")]
public record ParkingSpotCreated(Guid ParkingSpotId) : IEvent;