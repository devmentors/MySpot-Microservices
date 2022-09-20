using Micro.Abstractions;

namespace MySpot.Services.Reservations.Application.Commands;

public record CreateWeeklyReservations(Guid UserId) : ICommand;