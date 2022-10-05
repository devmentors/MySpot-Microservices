using MySpot.Services.Reservations.Application.DTO;
using MySpot.Services.Reservations.Core.Entities;
using MySpot.Services.Reservations.Core.ValueObjects;
using MySpot.Services.Reservations.Infrastructure.DAL.ReadModels;

namespace MySpot.Services.Reservations.Infrastructure.DAL.Handlers;

internal static class Extensions
{
    public static WeeklyReservationsDto AsDto(this WeeklyReservationsReadModel entity)
        => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            From = new Week(entity.To).From.Value.UtcDateTime,
            To = entity.To.UtcDateTime,
            Reservations = entity.Reservations.Select(x => new ReservationDto()
            {
                Id = x.Id,
                ParkingSpotId = x.ParkingSpotId,
                Capacity = x.Capacity,
                Date = x.Date.UtcDateTime,
                State = x.State
            }).ToList()
        };
}