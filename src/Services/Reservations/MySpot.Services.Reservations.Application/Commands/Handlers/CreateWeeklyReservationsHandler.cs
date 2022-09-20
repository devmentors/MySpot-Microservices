using Micro.Handlers;
using Micro.Time;
using MySpot.Services.Reservations.Core.Factories;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.Types;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Application.Commands.Handlers;

internal sealed class CreateWeeklyReservationsHandler : ICommandHandler<CreateWeeklyReservations>
{
    private readonly IWeeklyReservationsFactory _weeklyReservationsFactory;
    private readonly IWeeklyReservationsRepository _weeklyReservationsRepository;
    private readonly IClock _clock;

    public CreateWeeklyReservationsHandler(IWeeklyReservationsFactory weeklyReservationsFactory,
        IWeeklyReservationsRepository weeklyReservationsRepository, IClock clock)
    {
        _weeklyReservationsFactory = weeklyReservationsFactory;
        _weeklyReservationsRepository = weeklyReservationsRepository;
        _clock = clock;
    }

    public async Task HandleAsync(CreateWeeklyReservations command, CancellationToken cancellationToken = default)
    {
        var userId = new UserId(command.UserId);
        var week = new Week(_clock.Current());
        var weeklyReservations = await _weeklyReservationsRepository.GetForCurrentWeekAsync(userId, cancellationToken);
        if (weeklyReservations is not null)
        {
            return;
        }

        weeklyReservations = await _weeklyReservationsFactory.CreateAsync(userId, week, cancellationToken);
        await _weeklyReservationsRepository.AddAsync(weeklyReservations, cancellationToken);
    }
}