using Micro.Exceptions;

namespace MySpot.Services.Availability.Application.Exceptions;

public class ReservationNotFoundException : CustomException
{
    public DateTimeOffset Date { get; }

    public ReservationNotFoundException(DateTimeOffset date) : base($"Reservation for date: {date} was not found.")
        => Date = date;
}