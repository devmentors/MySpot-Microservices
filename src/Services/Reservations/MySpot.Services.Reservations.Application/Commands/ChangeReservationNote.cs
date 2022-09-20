using Micro.Abstractions;

namespace MySpot.Services.Reservations.Application.Commands;

public record ChangeReservationNote(Guid WeeklyReservationsId, Guid ReservationId, Guid UserId, string Note) : ICommand;