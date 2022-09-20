using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Commands;

[Message("availability", "delete_resource", "availability.delete_resource")]
public record DeleteResource(Guid ResourceId) : ICommand;