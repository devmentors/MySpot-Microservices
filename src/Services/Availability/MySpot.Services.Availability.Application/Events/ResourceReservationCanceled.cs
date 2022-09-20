using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Availability.Application.Events;

[Message("availability", "resource_reservation_canceled")]
public record ResourceReservationCanceled(Guid ResourceId, DateTimeOffset Date) : IEvent;