using Micro.Exceptions;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Exception;

public sealed class InvalidReservationDateException : CustomException
{
    public DateTimeOffset Date { get; }

    public InvalidReservationDateException(Date date) : base($"Reservation date is invalid: {date}.")
    {
        Date = date;
    }
}