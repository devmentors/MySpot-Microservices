using Micro.Handlers;
using MySpot.Services.Reservations.Application.Exceptions;
using MySpot.Services.Reservations.Core.Repository;

namespace MySpot.Services.Reservations.Application.Commands.Handlers;

internal sealed class RemoveReservationHandler : ICommandHandler<RemoveReservation>
{
    private readonly IWeeklyReservationsRepository _weeklyReservationsRepository;

    public RemoveReservationHandler(IWeeklyReservationsRepository weeklyReservationsRepository)
    {
        _weeklyReservationsRepository = weeklyReservationsRepository;
    }

    public async Task HandleAsync(RemoveReservation command, CancellationToken cancellationToken = default)
    {
        var (reservationId, userId) = command;
        var weeklyReservations = await _weeklyReservationsRepository.GetForCurrentWeekAsync(userId, cancellationToken);
        if (weeklyReservations is null)
        {
            throw new WeeklyReservationsForCurrentWeekNotFoundException();
        }

        weeklyReservations.RemoveReservation(reservationId);
        await _weeklyReservationsRepository.UpdateAsync(weeklyReservations, cancellationToken);
    }
}