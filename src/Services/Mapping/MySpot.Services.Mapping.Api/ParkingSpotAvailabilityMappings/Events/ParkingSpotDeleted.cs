using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Events;

[Message("parking_spots", "parking_spot_deleted", "mapping.parking_spot_deleted")]
public record ParkingSpotDeleted(Guid ParkingSpotId) : IEvent;