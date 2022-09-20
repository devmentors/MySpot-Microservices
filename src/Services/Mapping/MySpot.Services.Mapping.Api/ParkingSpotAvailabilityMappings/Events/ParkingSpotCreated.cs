using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Events;

[Message("parking_spots", "parking_spot_created", "mapping.parking_spot_created")]
public record ParkingSpotCreated(Guid ParkingSpotId) : IEvent;