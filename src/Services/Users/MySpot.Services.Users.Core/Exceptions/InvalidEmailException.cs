using Micro.Exceptions;

namespace MySpot.Services.Users.Core.Exceptions;

internal class InvalidEmailException : CustomException
{
    public string Email { get; }

    public InvalidEmailException(string email) : base($"Email is invalid: '{email}'.")
    {
        Email = email;
    }
}