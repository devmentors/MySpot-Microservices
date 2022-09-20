using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Events.External;

[Message("parking_spots", "parking_spot_created", "availability.parking_spot_created")]
public record ParkingSpotCreated(Guid ParkingSpotId) : IEvent;