using Micro.Handlers;
using Micro.Messaging.Brokers;
using MySpot.Services.Reservations.Application.Events;
using MySpot.Services.Reservations.Application.Exceptions;
using MySpot.Services.Reservations.Core.Repository;

namespace MySpot.Services.Reservations.Application.Commands.Handlers;

internal sealed class RemoveReservationHandler : ICommandHandler<RemoveReservation>
{
    private readonly IWeeklyReservationsRepository _weeklyReservationsRepository;
    private readonly IMessageBroker _messageBroker;

    public RemoveReservationHandler(IWeeklyReservationsRepository weeklyReservationsRepository,
        IMessageBroker messageBroker)
    {
        _weeklyReservationsRepository = weeklyReservationsRepository;
        _messageBroker = messageBroker;
    }

    public async Task HandleAsync(RemoveReservation command, CancellationToken cancellationToken = default)
    {
        var (reservationId, userId) = command;
        var weeklyReservations = await _weeklyReservationsRepository.GetForCurrentWeekAsync(userId, cancellationToken);
        if (weeklyReservations is null)
        {
            throw new WeeklyReservationsForCurrentWeekNotFoundException();
        }

        var reservation = weeklyReservations.RemoveReservation(reservationId);
        await _weeklyReservationsRepository.UpdateAsync(weeklyReservations, cancellationToken);
        await _messageBroker.SendAsync(new ParkingSpotReservationRemoved(reservation.Id, reservation.ParkingSpotId,
            reservation.Date), cancellationToken);
    }
}