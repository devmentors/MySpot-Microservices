using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Events;

[Message("availability", "resource_reservation_failed")]
public record ResourceReservationFailed(Guid ResourceId, DateTimeOffset Date, string Reason, string Code) : IEvent;