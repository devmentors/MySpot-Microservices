using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Saga.Api.Messages;

[Message("reservations", "remove_reservation")]
public record RemoveReservation(Guid ReservationId, Guid UserId) : ICommand;