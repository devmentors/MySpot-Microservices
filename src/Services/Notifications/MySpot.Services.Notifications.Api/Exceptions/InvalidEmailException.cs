using Micro.Exceptions;

namespace MySpot.Services.Notifications.Api.Exceptions;

internal sealed class InvalidEmailException : CustomException
{
    public string Email { get; }

    public InvalidEmailException(string email) : base($"Email is invalid: '{email}'.")
    {
        Email = email;
    }
}