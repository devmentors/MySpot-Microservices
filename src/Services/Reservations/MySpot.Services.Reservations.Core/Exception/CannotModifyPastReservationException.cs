using Micro.Exceptions;
using MySpot.Services.Reservations.Core.ValueObjects;

namespace MySpot.Services.Reservations.Core.Exception;

public sealed class CannotModifyPastReservationException : CustomException
{
    public Date Date { get; }

    public CannotModifyPastReservationException(Date date) 
        : base($"Cannot modify reservation with date: {date} which is past.")
    {
        Date = date;
    }
}