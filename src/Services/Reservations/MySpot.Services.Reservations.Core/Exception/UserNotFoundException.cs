using Micro.Exceptions;

namespace MySpot.Services.Reservations.Core.Exception;

internal sealed class UserNotFoundException : CustomException
{
    public Guid UserId { get; }

    public UserNotFoundException(Guid userId) : base($"User with ID: '{userId}' was not found.")
    {
        UserId = userId;
    }
}