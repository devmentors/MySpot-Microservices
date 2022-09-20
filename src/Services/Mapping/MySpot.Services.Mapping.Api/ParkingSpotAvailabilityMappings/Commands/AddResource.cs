using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Mapping.Api.ParkingSpotAvailabilityMappings.Commands;

[Message("availability", "add_resource")]
public record AddResource(Guid ResourceId, int Capacity, IEnumerable<string> Tags) : ICommand;