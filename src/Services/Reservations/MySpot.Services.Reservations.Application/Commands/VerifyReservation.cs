using Micro.Abstractions;

namespace MySpot.Services.Reservations.Application.Commands;

public record VerifyReservation(Guid WeeklyReservationsId, Guid ReservationId, Guid UserId) : ICommand;