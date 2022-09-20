using Micro.Exceptions;

namespace MySpot.Services.Reservations.Core.Exception;

public sealed class NoReservationPolicyFoundException : CustomException
{
    public string Participant { get; }

    public NoReservationPolicyFoundException(string participant) 
        : base($"No reservation policy found for participant: {participant}")
    {
        Participant = participant;
    }
}