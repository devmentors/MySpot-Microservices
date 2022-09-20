using Micro.Abstractions;

namespace MySpot.Services.Reservations.Application.Commands;

public record MakeReservationIncorrect(Guid WeeklyReservationsId, Guid ReservationId, Guid UserId) : ICommand;