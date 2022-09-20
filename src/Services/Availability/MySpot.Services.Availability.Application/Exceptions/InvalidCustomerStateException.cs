using Micro.Exceptions;

namespace MySpot.Services.Availability.Application.Exceptions;

public class InvalidCustomerStateException : CustomException
{
    public Guid Id { get; }
    public string State { get; }

    public InvalidCustomerStateException(Guid id, string state)
        : base($"Customer with id: {id} has invalid state: {state}.")
        => (Id, State) = (id, state);
}