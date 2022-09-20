using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Events;

[Message("availability", "resource_added")]
public record ResourceAdded(Guid ResourceId) : IEvent;