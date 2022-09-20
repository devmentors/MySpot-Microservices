using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Saga.Api.Messages;

[Message("availability", "resource_reservation_failed", "saga.resource_reservation_failed")]
public record ResourceReservationFailed(Guid ResourceId, DateTimeOffset Date, string Reason, string Code) : IEvent;