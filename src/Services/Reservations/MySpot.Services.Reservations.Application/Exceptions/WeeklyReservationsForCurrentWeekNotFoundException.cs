using Micro.Exceptions;

namespace MySpot.Services.Reservations.Application.Exceptions;

public sealed class WeeklyReservationsForCurrentWeekNotFoundException : CustomException
{
    public WeeklyReservationsForCurrentWeekNotFoundException() 
        : base("Reservations for current week not found.")
    {
    }
}