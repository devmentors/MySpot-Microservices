using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Commands;

[Message("availability", "delete_resource")]
public record DeleteResource(Guid ResourceId) : ICommand;