using Micro.Exceptions;

namespace MySpot.Services.Reservations.Core.Exception;

public class InvalidCapacityException : CustomException
{
    public int Capacity { get; }

    public InvalidCapacityException(int capacity) : base($"Reservation capacity: {capacity} is invalid")
        => Capacity = capacity;
}