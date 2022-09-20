using Micro.Abstractions;
using MySpot.Services.Reservations.Application.DTO;

namespace MySpot.Services.Reservations.Application.Queries;

public class GetWeeklyReservations : IQuery<WeeklyReservationsDto?>
{
    public Guid UserId { get; set; }
    public DateTime? Date { get; set; }
}