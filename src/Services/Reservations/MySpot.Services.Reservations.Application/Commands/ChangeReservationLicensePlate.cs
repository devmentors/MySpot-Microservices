using Micro.Abstractions;

namespace MySpot.Services.Reservations.Application.Commands;

public record ChangeReservationLicensePlate(Guid WeeklyReservationsId, Guid ReservationId, Guid UserId, string Note) : ICommand;