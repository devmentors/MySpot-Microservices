using Micro.Abstractions;
using Micro.Attributes;

namespace MySpot.Services.Reservations.Application.Commands;

[Message("reservations", "remove_reservation", "reservations.remove_reservation")]
public record RemoveReservation(Guid ReservationId, Guid UserId) : ICommand;