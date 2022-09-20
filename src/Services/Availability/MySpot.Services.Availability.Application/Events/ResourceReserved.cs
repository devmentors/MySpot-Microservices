using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Events;

[Message("availability", "resource_reserved")]
public record ResourceReserved(Guid ResourceId, DateTimeOffset Date) : IEvent;