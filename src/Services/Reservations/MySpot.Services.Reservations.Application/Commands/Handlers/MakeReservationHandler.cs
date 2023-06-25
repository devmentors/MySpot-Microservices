using Micro.Handlers;
using Micro.Time;
using MySpot.Services.Reservations.Core.DomainServices;
using MySpot.Services.Reservations.Core.Factories;
using MySpot.Services.Reservations.Core.Repository;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Application.Commands.Handlers;

internal sealed class MakeReservationHandler : ICommandHandler<MakeReservation>
{
    private readonly IWeeklyReservationsRepository _weeklyReservationsRepository;
    private readonly IWeeklyReservationsService _weeklyReservationsService;
    private readonly IWeeklyReservationsFactory _weeklyReservationsFactory;
    private readonly IClock _clock;

    public MakeReservationHandler(IWeeklyReservationsRepository weeklyReservationsRepository,
        IWeeklyReservationsService weeklyReservationsService, IWeeklyReservationsFactory weeklyReservationsFactory,
        IClock clock)
    {
        _weeklyReservationsRepository = weeklyReservationsRepository;
        _weeklyReservationsService = weeklyReservationsService;
        _weeklyReservationsFactory = weeklyReservationsFactory;
        _clock = clock;
    }

    public async Task HandleAsync(MakeReservation command, CancellationToken cancellationToken = default)
    {
        var (userId, parkingSpotId, capacity, licensePlate, date, note) = command;
        _ = new LicensePlate(licensePlate);
        _ = new Capacity(capacity);
        _ = new Date(date);

        var lastWeekReservations = await _weeklyReservationsRepository.GetForLastWeekAsync(userId, cancellationToken);
        var currentReservations = await _weeklyReservationsRepository.GetForCurrentWeekAsync(userId, cancellationToken);
        if (currentReservations is null)
        {
            currentReservations = await _weeklyReservationsFactory.CreateAsync(userId, new Week(_clock.Current()), cancellationToken);
            await _weeklyReservationsRepository.AddAsync(currentReservations, cancellationToken);
        }

        var reservation = _weeklyReservationsService.Reserve(currentReservations, lastWeekReservations, parkingSpotId,
            capacity, licensePlate, date, note);
        await _weeklyReservationsRepository.UpdateAsync(currentReservations, cancellationToken);
    }
}