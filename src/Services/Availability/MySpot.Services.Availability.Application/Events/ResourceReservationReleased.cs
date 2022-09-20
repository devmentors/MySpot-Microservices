using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Events;

[Message("availability", "resource_reservation_released")]
public record ResourceReservationReleased(Guid ResourceId, DateTimeOffset Date) : IEvent;