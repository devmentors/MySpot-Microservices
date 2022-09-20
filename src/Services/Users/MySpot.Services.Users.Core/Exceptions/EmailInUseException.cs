using Micro.Exceptions;

namespace MySpot.Services.Users.Core.Exceptions;

internal class EmailInUseException : CustomException
{
    public EmailInUseException() : base("Email is already in use.")
    {
    }
}