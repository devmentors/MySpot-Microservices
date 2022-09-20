using Micro.Exceptions;
using MySpot.Services.Reservations.Core.Types;

namespace MySpot.Services.Reservations.Core.Exception;

public sealed class ReservationNotFoundException : CustomException
{
    public ReservationId ReservationId { get; }

    public ReservationNotFoundException(ReservationId reservationId) 
        : base($"Couldn't find reservation with ID: {reservationId}")
    {
        ReservationId = reservationId;
    }
}