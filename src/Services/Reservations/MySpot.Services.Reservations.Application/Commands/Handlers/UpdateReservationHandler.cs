using Micro.Handlers;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.Types;

namespace MySpot.Services.Reservations.Application.Commands.Handlers;

internal sealed class UpdateReservationHandler : 
    ICommandHandler<ChangeReservationNote>,
    ICommandHandler<ChangeReservationLicensePlate>,
    ICommandHandler<MakeReservationIncorrect>,
    ICommandHandler<VerifyReservation>
{
    private readonly IWeeklyReservationsRepository _weeklyReservationsRepository;

    public UpdateReservationHandler(IWeeklyReservationsRepository weeklyReservationsRepository)
    {
        _weeklyReservationsRepository = weeklyReservationsRepository;
    }

    public async Task HandleAsync(ChangeReservationNote command, CancellationToken cancellationToken = default)
    {
        var (weeklyReservationsId, reservationId, userId, note) = command;
        await UpdateReservationsAsync(weeklyReservationsId,  userId,
            x => x.ChangeReservationsNote(reservationId, note), cancellationToken);
    }

    public async Task HandleAsync(ChangeReservationLicensePlate command, CancellationToken cancellationToken = default)
    {
        var (weeklyReservationsId, reservationId, userId, licensePlate) = command;
        await UpdateReservationsAsync(weeklyReservationsId, userId,
            x => x.ChangeLicensePlate(reservationId, licensePlate), cancellationToken);
    }

    public async Task HandleAsync(MakeReservationIncorrect command, CancellationToken cancellationToken = default)
    {
        var (weeklyReservationsId, reservationId, userId) = command;
        await UpdateReservationsAsync(weeklyReservationsId, userId,
            x => x.MarkReservationAsIncorrect(reservationId), cancellationToken);
    }

    public async Task HandleAsync(VerifyReservation command, CancellationToken cancellationToken = default)
    {
        var (weeklyReservationsId, reservationId, userId) = command;
        await UpdateReservationsAsync(weeklyReservationsId, userId,
            x => x.MarkReservationAsVerified(reservationId), cancellationToken);
    }

    private async Task UpdateReservationsAsync(AggregateId weeklyReservationsId, UserId userId,
        Action<WeeklyReservations> update, CancellationToken cancellationToken = default)
    {
        var weeklyReservations = await _weeklyReservationsRepository.GetForCurrentWeekAsync(userId, cancellationToken);
        if (weeklyReservations is null)
        {
            return;
        }
        
        update(weeklyReservations);
        await _weeklyReservationsRepository.UpdateAsync(weeklyReservations, cancellationToken);
    }
}