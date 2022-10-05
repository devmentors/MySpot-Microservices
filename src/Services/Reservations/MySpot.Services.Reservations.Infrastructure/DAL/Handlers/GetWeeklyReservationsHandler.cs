using Micro.Handlers;
using Micro.Time;
using Microsoft.EntityFrameworkCore;
using MySpot.Services.Reservations.Application.DTO;
using MySpot.Services.Reservations.Application.Queries;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.ValueObjects;
using MySpot.Services.Reservations.Infrastructure.DAL.ReadModels;

namespace MySpot.Services.Reservations.Infrastructure.DAL.Handlers;

internal sealed class GetWeeklyReservationsHandler : IQueryHandler<GetWeeklyReservations, WeeklyReservationsDto?>
{
    private readonly DbSet<WeeklyReservationsReadModel> _weeklyReservations;
    private readonly IClock _clock;

    public GetWeeklyReservationsHandler(ReservationsReadDbContext context, IClock clock)
    {
        _weeklyReservations = context.WeeklyReservations;
        _clock = clock;
    }

    public Task<WeeklyReservationsDto?> HandleAsync(GetWeeklyReservations query,
        CancellationToken cancellationToken = default)
    {
        var week = new Week(query.Date ?? _clock.Current());

        return _weeklyReservations
            .Include(x => x.Reservations)
            .AsNoTracking()
            .Where(x => x.UserId == query.UserId && x.To == week.To.Value)
            .Select(x => x.AsDto())
            .SingleOrDefaultAsync(cancellationToken);
    }
}