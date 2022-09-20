using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Commands;

[Message("availability", "add_resource", "availability.add_resource")]
public record AddResource(Guid ResourceId, int Capacity, IEnumerable<string> Tags) : ICommand;