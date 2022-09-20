using MySpot.Services.Reservations.Application.DTO;
using MySpot.Services.Reservations.Core.Entities;

namespace MySpot.Services.Reservations.Infrastructure.DAL.Handlers;

internal static class Extensions
{
    public static WeeklyReservationsDto AsDto(this WeeklyReservations entity)
        => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            Reservations = entity.Reservations.Select(x => new ReservationDto
            {
                Id = x.Id,
                ParkingSpotId = x.ParkingSpotId,
                Capacity = x.Capacity,
                Date = x.Date.Value.DateTime,
                LicensePlate = x.LicensePlate,
                State = x.State,
                Note = x.Note
            }).ToList()
        };
}